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

    private string _selectedMode = "501";
    private bool _doubleOut = true;
    private int _legsBestOf = 3;
    private List<string> _selectedPlayerIds = new();
    private bool _showSetup = false;
    private bool _showModeSelect = true;
    private bool _showDartboard = false;
    private bool _showGameOver = false;
    private Game? _game;
    private List<Dart> _currentDarts = new();
    private int _currentDartIdx = 0;
    private int _multiplier = 1; // 1=single, 2=double, 3=triple

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

        _game = GameLogic.CreateGame(_selectedMode, _selectedPlayerIds, State.Players, _doubleOut, _legsBestOf, settings: State.Settings);
        await State.SetActiveGame(_game);
        _showSetup = false;
        _showDartboard = true;
        _currentDarts = new();
        _currentDartIdx = 0;
        _multiplier = 1;
        await Sound.PlaySfx("showdown", State.Settings);
        State.Notify();
    }

    private void SetMultiplier(int m) => _multiplier = m;

    private async Task RecordDart(int baseValue, string label)
    {
        if (_game == null || _game.Finished) return;
        if (_currentDartIdx >= 3) return;

        int value = baseValue * _multiplier;
        bool isDouble = _multiplier == 2;
        bool isTriple = _multiplier == 3;
        bool isBull = label == "Bull" || label == "Bullseye";

        string dartLabel = label;
        if (_multiplier == 2 && baseValue > 0) dartLabel = "D" + baseValue;
        else if (_multiplier == 3 && baseValue > 0) dartLabel = "T" + baseValue;

        var dart = new Dart { Value = value, Label = dartLabel, IsDouble = isDouble, IsTriple = isTriple, IsBull = isBull };
        _currentDarts.Add(dart);
        _currentDartIdx++;
        _multiplier = 1;

        await Sound.PlaySfx("hit", State.Settings);

        if (_currentDartIdx >= 3)
        {
            await SubmitVisit();
        }
    }

    private async Task RecordBull()
    {
        if (_game == null || _game.Finished) return;
        if (_currentDartIdx >= 3) return;

        int value = _multiplier == 2 ? 50 : 25;
        bool isBull = true;
        bool isDouble = _multiplier == 2;

        string dartLabel = _multiplier == 2 ? "Bullseye" : "Bull";

        var dart = new Dart { Value = value, Label = dartLabel, IsDouble = isDouble, IsBull = isBull };
        _currentDarts.Add(dart);
        _currentDartIdx++;
        _multiplier = 1;

        await Sound.PlaySfx("hit", State.Settings);

        if (_currentDartIdx >= 3)
        {
            await SubmitVisit();
        }
    }

    private async Task RecordMiss()
    {
        if (_game == null || _game.Finished) return;
        if (_currentDartIdx >= 3) return;

        var dart = new Dart { Value = 0, Label = "Miss" };
        _currentDarts.Add(dart);
        _currentDartIdx++;
        _multiplier = 1;

        if (_currentDartIdx >= 3)
        {
            await SubmitVisit();
        }
    }

    private async Task UndoDart()
    {
        if (_currentDarts.Count > 0)
        {
            _currentDarts.RemoveAt(_currentDarts.Count - 1);
            _currentDartIdx--;
            _multiplier = 1;
        }
    }

    private async Task SubmitVisit()
    {
        if (_game == null || _currentDarts.Count == 0) return;

        var player = _game.Players[_game.CurrentIdx];
        var scored = _currentDarts.Sum(d => d.Value);
        var remaining = player.Score - scored;
        var bust = false;

        if (!_game.Atc && !_game.Killer && !_game.Party && _game.StartScore > 0)
        {
            if (remaining < 0 || (_game.DoubleOut && remaining == 0 && !_currentDarts.Any(d => d.IsDouble || d.IsBull)))
            {
                bust = true;
                remaining = player.Score;
            }
        }

        var visit = new Visit
        {
            Darts = new List<Dart>(_currentDarts),
            Scored = bust ? 0 : scored,
            Remaining = _game.Atc ? 0 : Math.Max(0, remaining),
            Bust = bust,
            Date = DateTime.UtcNow
        };

        player.Visits.Add(visit);
        player.DartsThrown += _currentDarts.Count;

        if (_game.Atc)
        {
            var targetIdx = player.Visits.Sum(v => v.Hits);
            var hits = 0;
            foreach (var dart in _currentDarts)
            {
                if (targetIdx + hits < GameModes.AtcTargets.Length)
                {
                    var target = GameModes.AtcTargets[targetIdx + hits];
                    if (target == -1 && dart.IsBull) hits++;
                    else if (target != -1)
                    {
                        var dartNum = dart.Value / (dart.IsTriple ? 3 : dart.IsDouble ? 2 : 1);
                        if (dartNum == target) hits++;
                    }
                }
            }
            visit.Hits = hits;
            visit.EndIdx = player.Visits.Sum(v => v.Hits);

            if (visit.EndIdx >= GameModes.AtcTargets.Length)
            {
                player.Done = true;
                _game.Finished = true;
                _game.Winner = player.Id;
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
                    var topScore = _game.Players.Max(p => p.Visits.Sum(v => v.Scored));
                    var winner = _game.Players.First(p => p.Visits.Sum(v => v.Scored) == topScore);
                    _game.Winner = winner.Id;
                    await FinishGame();
                    return;
                }
            }
        }

        _game.CurrentIdx = (_game.CurrentIdx + 1) % _game.Players.Count;
        while (_game.Players[_game.CurrentIdx].Done)
        {
            if (_game.Players.All(p => p.Done)) break;
            _game.CurrentIdx = (_game.CurrentIdx + 1) % _game.Players.Count;
        }

        _currentDarts = new();
        _currentDartIdx = 0;
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
                Done = p.Done
            }).ToList(),
            Winner = _game.Winner,
            Date = _game.Date,
            Practice = _game.Practice,
            Atc = _game.Atc,
            Party = _game.Party
        };

        State.Games.Add(record);
        await State.SetGames(State.Games);

        if (!record.Practice && _game.Winner != null)
        {
            var winner = State.Players.FirstOrDefault(p => p.Id == _game.Winner);
            if (winner != null)
            {
                winner.Xp += State.Settings.XpConfig.Win;
                var (xp, level) = GameLogic.LevelFromXp(winner.Xp, State.Settings);
                winner.Level = level;
                await State.SetPlayers(State.Players);
            }
        }

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
        _showModeSelect = true;
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
        _showModeSelect = false;
        _showSetup = true;
    }

    private void BackToModeSelect()
    {
        _showSetup = false;
        _showModeSelect = true;
    }

    private string CheckoutHint(int remaining)
    {
        if (Checkouts.Table.TryGetValue(remaining, out var darts))
            return string.Join(" · ", darts);
        return "";
    }
}
