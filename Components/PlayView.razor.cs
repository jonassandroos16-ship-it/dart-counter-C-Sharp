using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;

namespace dart_counter.Components;

public partial class PlayView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;
    [Inject] SoundService Sound { get; set; } = default!;
    [CascadingParameter] App? AppRef { get; set; }

    private string _selectedMode = "501";
    private bool _doubleOut = true;
    private int _legsBestOf = 3;
    private List<string> _selectedPlayerIds = new();
    private bool _showSetup = false;
    private bool _showDartboard = false;
    private bool _showShowdown = false;
    private bool _showGameOver = false;
    private bool _teamMode = false;
    private bool _powerUpsEnabled = false;
    private Dictionary<int, int> _teamAssignment = new();
    private Game? _game;
    private int _multiplier = 1;

    protected override void OnInitialized()
    {
        State.OnChange += StateOnChange;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private async Task StartGame()
    {
        if (_selectedPlayerIds.Count == 0)
        {
            Toast.Show("Select at least one player!");
            return;
        }

        int[]? teams = null;
        if (_teamMode)
        {
            teams = new int[_selectedPlayerIds.Count];
            for (int i = 0; i < _selectedPlayerIds.Count; i++)
                teams[i] = _teamAssignment.GetValueOrDefault(i, 0);
        }

        _game = GameLogic.CreateGame(_selectedMode, _selectedPlayerIds, State.Players, _doubleOut, _legsBestOf, _teamMode, teams, _powerUpsEnabled, State.Settings);
        await State.SetActiveGame(_game);
        _showSetup = false;
        _showShowdown = true;
        _showDartboard = false;
        _showGameOver = false;
        await Sound.PlaySfx("showdown", State.Settings);
        State.Notify();
    }

    private void CloseShowdown()
    {
        _showShowdown = false;
        _showDartboard = true;
        _ = Sound.PlaySfx("showdown_close", State.Settings);
        State.Notify();
    }

    private void SetMultiplier(int m) => _multiplier = m;

    private async Task RecordDart(int baseValue)
    {
        if (_game == null || _game.Finished) return;
        var player = _game.Players[_game.CurrentIdx];
        var darts = _game.Darts ?? new();
        var maxDarts = player._oneDartNext ? 1 : 3;
        if (darts.Count >= maxDarts) return;

        int value = baseValue * _multiplier;
        bool isDouble = _multiplier == 2;
        bool isTriple = _multiplier == 3;
        string label = isDouble ? "D" + baseValue : isTriple ? "T" + baseValue : baseValue.ToString();

        var dart = new Dart { Value = value, Label = label, Base = baseValue, Mult = _multiplier, IsDouble = isDouble, IsTriple = isTriple };
        darts.Add(dart);
        _game.Darts = darts;

        if (_game.PowerUpsEnabled && player.PowerUpId != null)
            GameLogic.AddCharge(player, GameLogic.ChargeForDart(dart, State.Settings), State.Settings);

        await Sound.PlayHit(State.Settings, value);
        _multiplier = 1;

        if (darts.Count >= maxDarts)
            await SubmitVisit();
        else
            State.Notify();
    }

    private async Task RecordBull()
    {
        if (_game == null || _game.Finished) return;
        var player = _game.Players[_game.CurrentIdx];
        var darts = _game.Darts ?? new();
        var maxDarts = player._oneDartNext ? 1 : 3;
        if (darts.Count >= maxDarts) return;

        var dart = new Dart { Value = 25, Label = "Bull", Base = 0, Mult = 1, IsBull = true, IsOuterBull = true };
        darts.Add(dart);
        _game.Darts = darts;
        if (_game.PowerUpsEnabled && player.PowerUpId != null)
            GameLogic.AddCharge(player, GameLogic.ChargeForDart(dart, State.Settings), State.Settings);
        await Sound.PlayHit(State.Settings, 25);
        if (darts.Count >= maxDarts) await SubmitVisit(); else State.Notify();
    }

    private async Task RecordBullseye()
    {
        if (_game == null || _game.Finished) return;
        var player = _game.Players[_game.CurrentIdx];
        var darts = _game.Darts ?? new();
        var maxDarts = player._oneDartNext ? 1 : 3;
        if (darts.Count >= maxDarts) return;

        var dart = new Dart { Value = 50, Label = "Bullseye", Base = 0, Mult = 2, IsBull = true, IsDouble = true };
        darts.Add(dart);
        _game.Darts = darts;
        if (_game.PowerUpsEnabled && player.PowerUpId != null)
            GameLogic.AddCharge(player, GameLogic.ChargeForDart(dart, State.Settings), State.Settings);
        await Sound.PlayHit(State.Settings, 50);
        if (darts.Count >= maxDarts) await SubmitVisit(); else State.Notify();
    }

    private async Task RecordMiss()
    {
        if (_game == null || _game.Finished) return;
        var player = _game.Players[_game.CurrentIdx];
        var darts = _game.Darts ?? new();
        var maxDarts = player._oneDartNext ? 1 : 3;
        if (darts.Count >= maxDarts) return;

        darts.Add(new Dart { Value = 0, Label = "Miss" });
        _game.Darts = darts;
        if (darts.Count >= maxDarts) await SubmitVisit(); else State.Notify();
    }

    private async Task UndoDart()
    {
        if (_game == null) return;
        var darts = _game.Darts ?? new();
        if (darts.Count > 0) { darts.RemoveAt(darts.Count - 1); _game.Darts = darts; _multiplier = 1; State.Notify(); }
    }

    private async Task SubmitVisit()
    {
        if (_game == null) return;
        var darts = _game.Darts ?? new();
        if (darts.Count == 0) return;
        var player = _game.Players[_game.CurrentIdx];

        if (player._frozenNext)
        {
            player._frozenNext = false;
            var visit = new Visit { Darts = new(darts), Scored = 0, Remaining = player.Score, Bust = false, Date = DateTime.UtcNow, Leg = _game.CurrentLeg };
            player.Visits.Add(visit);
            player.DartsThrown += darts.Count;
            _game.Darts = new();
            await AdvanceTurn();
            return;
        }

        int scored = darts.Sum(d => d.Value);

        if (player._surgeNext && !player._surgeArmed) scored *= 2;
        if (player._crippledNext) scored = scored / 2;
        if (player._bullseyeFrenzy)
        {
            scored += darts.Where(d => d.IsBull).Sum(d => d.Value);
        }
        if (player._hotStreak)
        {
            int bonus = 0;
            for (int i = 0; i < darts.Count; i++) bonus += i * 5;
            scored += bonus;
        }

        int remaining = player.Score - scored;
        bool bust = false;
        int checkout = 0;

        if (!_game.Atc && !_game.Killer && !_game.Party && _game.StartScore > 0)
        {
            if (remaining < 0 || (_game.DoubleOut && remaining == 0 && !darts.Any(d => d.IsDouble || d.IsBull)))
            {
                if (player._luckyMiss)
                {
                    player._luckyMiss = false;
                    scored = 0;
                    remaining = player.Score;
                }
                else
                {
                    bust = true;
                    remaining = player.Score;
                }
            }
            else if (remaining == 0)
            {
                checkout = scored;
            }
        }

        var v = new Visit
        {
            Darts = new(darts),
            Scored = bust ? 0 : scored,
            Remaining = _game.Atc ? 0 : Math.Max(0, remaining),
            Bust = bust,
            Date = DateTime.UtcNow,
            Leg = _game.CurrentLeg,
            Checkout = checkout
        };

        player.Visits.Add(v);
        player.DartsThrown += darts.Count;

        player._surgeNext = player._surgeNext && player._surgeArmed;
        player._surgeArmed = false;
        player._crippledNext = false;
        player._frozenNext = false;
        player._bullseyeFrenzy = false;
        player._hotStreak = false;
        player._oneDartNext = false;
        if (player._shieldTurns > 0) player._shieldTurns--;

        if (!bust && scored > 0 && !v.Atc && State.Settings.Popups.Scores)
        {
            var popup = ScorePopups.Scores.FirstOrDefault(p => scored >= p.Min);
            if (popup != null && AppRef != null)
                AppRef.ShowPopup(popup.Emoji, popup.Title, popup.Sub);
        }

        if (!bust && remaining > 0 && remaining <= 200 && State.Settings.Popups.Milestones && AppRef != null)
        {
            var ms = ScorePopups.Milestones.FirstOrDefault(p => remaining <= p.Min);
            if (ms != null) AppRef.ShowPopup(ms.Emoji, ms.Title, ms.Sub);
        }

        if (_game.Atc)
        {
            var currentIdx = player.Visits.Sum(v2 => v2.Hits);
            var hits = 0;
            foreach (var dart in darts)
            {
                if (currentIdx + hits < GameModes.AtcTargets.Length)
                {
                    var target = GameModes.AtcTargets[currentIdx + hits];
                    if (target == -1 && dart.IsBull) hits++;
                    else if (target != -1 && dart.Base == target) hits++;
                }
            }
            v.Hits = hits;
            v.EndIdx = player.Visits.Sum(v2 => v2.Hits);
            v.Atc = true;

            if (v.EndIdx >= GameModes.AtcTargets.Length)
            {
                player.Done = true;
                _game.Finished = true;
                _game.Winner = player.Id;
                await FinishGame();
                return;
            }
        }
        else if (_game.Killer)
        {
            var kn = player.KillerNumber ?? 0;
            foreach (var dart in darts)
            {
                if (dart.Base == kn)
                {
                    if ((player.KillerHits ?? 0) < 5)
                    {
                        player.KillerHits = (player.KillerHits ?? 0) + 1;
                    }
                    else
                    {
                        foreach (var opp in _game.Players)
                        {
                            if (opp.Id == player.Id || opp.Eliminated == true) continue;
                            if (opp.KillerNumber == kn) continue;
                            if (dart.Base == opp.KillerNumber)
                            {
                                opp.Lives = (opp.Lives ?? 0) - 1;
                                player.Kills ??= new();
                                if (!player.Kills.Contains(opp.Id)) player.Kills.Add(opp.Id);
                                if (opp.Lives <= 0)
                                {
                                    opp.Eliminated = true;
                                    if (AppRef != null) AppRef.ShowKill(player.Name, opp.Name);
                                }
                            }
                        }
                    }
                }
            }
            var alive = _game.Players.Where(p => p.Eliminated != true).ToList();
            if (alive.Count <= 1)
            {
                _game.Finished = true;
                _game.Winner = alive.FirstOrDefault()?.Id;
                await FinishGame();
                return;
            }
        }
        else if (_game.IsBattle())
        {
            int totalDmg = 0;
            foreach (var dart in darts)
            {
                var target = _game.Players.FirstOrDefault(p => p.Id != player.Id && p.Defeated != true);
                if (target != null)
                {
                    int dmg = GameLogic.ComputeBattleDamage(dart, player, target, State.Settings);
                    target.Hp = Math.Max(0, (target.Hp ?? 0) - dmg);
                    totalDmg += dmg;
                    if (target.Hp <= 0 && target.Defeated != true)
                    {
                        target.Defeated = true;
                        if (AppRef != null) AppRef.ShowKill(player.Name, target.Name);
                    }
                }
            }
            player.DamageDealt = (player.DamageDealt ?? 0) + totalDmg;
            var alive = _game.Players.Where(p => p.Defeated != true).ToList();
            if (alive.Count <= 1)
            {
                _game.Finished = true;
                _game.Winner = alive.FirstOrDefault()?.Id;
                await FinishGame();
                return;
            }
        }
        else if (remaining == 0 && !bust)
        {
            player.LegsWon++;
            var legsToWin = (_game.LegsBestOf + 1) / 2;
            if (player.LegsWon >= legsToWin)
            {
                player.Done = true;
                _game.Finished = true;
                _game.Winner = player.Id;
                if (_game.TeamMode) _game.WinningTeam = player.Team;
                await FinishGame();
                return;
            }
            else
            {
                foreach (var p in _game.Players) { p.Score = _game.StartScore; p.Visits = new(); }
                _game.CurrentLeg++;
                _game.CurrentIdx = (_game.CurrentLeg - 1) % _game.Players.Count;
            }
        }
        else if (!_game.Practice && !_game.Party)
        {
            player.Score = remaining;
        }

        if (_game.HighScore())
        {
            var maxVisits = 7;
            if (player.Visits.Count >= maxVisits)
            {
                player.Done = true;
                var allDone = _game.Players.All(p => p.Done);
                if (allDone)
                {
                    _game.Finished = true;
                    var topScore = _game.Players.Max(p => p.Visits.Sum(v2 => v2.Scored));
                    var winners = _game.Players.Where(p => p.Visits.Sum(v2 => v2.Scored) == topScore).ToList();
                    if (winners.Count == 1) _game.Winner = winners[0].Id;
                    else { _game.Tied = true; _game.TiedPlayers = winners.Select(p => p.Id).ToList(); }
                    await FinishGame();
                    return;
                }
            }
        }

        await AdvanceTurn();
    }

    private async Task AdvanceTurn()
    {
        _game!.Darts = new();
        _multiplier = 1;
        _game.CurrentIdx = (_game.CurrentIdx + 1) % _game.Players.Count;
        while (_game.Players[_game.CurrentIdx].Done || _game.Players[_game.CurrentIdx].Eliminated == true || _game.Players[_game.CurrentIdx].Defeated == true)
        {
            if (_game.Players.All(p => p.Done || p.Eliminated == true || p.Defeated == true)) break;
            _game.CurrentIdx = (_game.CurrentIdx + 1) % _game.Players.Count;
        }
        await State.SetActiveGame(_game);
        State.Notify();
    }

    private async Task FinishGame()
    {
        if (_game == null) return;
        await State.SetActiveGame(_game);

        var record = new GameRecord
        {
            Id = _game.Id,
            Mode = _game.Mode,
            Players = _game.Players.Select(p => new GamePlayer
            {
                Id = p.Id, Name = p.Name, Color = p.Color,
                Score = p.Score, LegsWon = p.LegsWon,
                Visits = p.Visits, DartsThrown = p.DartsThrown,
                Done = p.Done, Team = p.Team,
                Lives = p.Lives, Eliminated = p.Eliminated,
                KillerNumber = p.KillerNumber, KillerHits = p.KillerHits, Kills = p.Kills,
                Hp = p.Hp, MaxHp = p.MaxHp, ArmorPct = p.ArmorPct, PowerPct = p.PowerPct,
                DamageDealt = p.DamageDealt, Defeated = p.Defeated
            }).ToList(),
            Winner = _game.Winner,
            Tied = _game.Tied,
            TiedPlayers = _game.TiedPlayers,
            Date = _game.Date,
            Practice = _game.Practice,
            Atc = _game.Atc,
            Party = _game.Party,
            TeamMode = _game.TeamMode,
            WinningTeam = _game.WinningTeam,
            TeamCount = _game.TeamCount,
            LegsBestOf = _game.LegsBestOf
        };

        State.Games.Add(record);
        await State.SetGames(State.Games);

        if (!record.Practice && _game.Winner != null)
        {
            var winner = State.Players.FirstOrDefault(p => p.Id == _game.Winner);
            if (winner != null)
            {
                var gp = _game.Players.First(p => p.Id == _game.Winner);
                int xpGained = GameLogic.AwardXp(winner, _game, gp, State.Settings);
                if (AppRef != null && xpGained > 0 && State.Settings.Popups.Xp)
                {
                    var (_, newLevel) = GameLogic.LevelFromXp(winner.Xp, State.Settings);
                    if (newLevel > winner.Level)
                    {
                        winner.Level = newLevel;
                        AppRef.ShowLevelUp(newLevel, winner.Name, xpGained, "Match Win");
                    }
                }
            }
        }

        foreach (var gp in _game.Players)
        {
            var p = State.Players.FirstOrDefault(pp => pp.Id == gp.Id);
            if (p == null) continue;
            if (_game.Winner != p.Id)
            {
                int xp = GameLogic.AwardXp(p, _game, gp, State.Settings);
            }
            var newTitles = GameLogic.CheckTitles(p, _game, State.Games, State.Settings);
            if (AppRef != null && State.Settings.Popups.Titles)
            {
                foreach (var tid in newTitles)
                {
                    var t = Titles.GetInfo(tid, State.Settings.CustomTitles);
                    if (t != null) AppRef.ShowTitleUnlock(t.Icon, t.Name, p.Name, t.Desc);
                }
            }
            GameLogic.CheckBadges(p, _game, State.Games);
        }

        await State.SetPlayers(State.Players);
        _showDartboard = false;
        _showGameOver = true;
        await Sound.PlaySfx("victory", State.Settings);
        State.Notify();
    }

    private async Task QuitGame()
    {
        await State.SetActiveGame(null);
        _game = null;
        _showDartboard = false;
        _showGameOver = false;
        _showSetup = false;
        _showShowdown = false;
        State.Notify();
    }

    private void TogglePlayer(string id)
    {
        if (_selectedPlayerIds.Contains(id)) _selectedPlayerIds.Remove(id);
        else _selectedPlayerIds.Add(id);
    }

    private void PickMode(string mode)
    {
        _selectedMode = mode;
        _showSetup = true;
    }

    private void BackToModeSelect()
    {
        _showSetup = false;
    }

    private string CheckoutHint(int remaining)
    {
        if (Checkouts.Table.TryGetValue(remaining, out var darts))
            return string.Join(" · ", darts);
        return "";
    }

    private async Task ActivatePowerUp()
    {
        if (_game == null) return;
        var player = _game.Players[_game.CurrentIdx];
        var result = GameLogic.ApplyPowerUp(_game, _game.CurrentIdx, State.Settings);
        Toast.Show(result.Message);
        await Sound.PlaySfx("powerup", State.Settings);
        if (result.ShieldBlocked && AppRef != null)
            AppRef.ShowPopup("🏰", "SHIELD BLOCKED!", $"{player.Name}'s Shield absorbed the attack!");
        await State.SetActiveGame(_game);
        State.Notify();
    }
}