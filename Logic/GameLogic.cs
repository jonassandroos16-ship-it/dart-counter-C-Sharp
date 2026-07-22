using dart_counter.Models;

namespace dart_counter.Logic;

public static class GameLogic
{
    public static Game CreateGame(string modeKey, List<string> playerIds, List<Player> players, bool doubleOut, int legsBestOf, bool teamMode = false, int[]? teamAssignment = null, bool powerUpsEnabled = false, Settings? settings = null)
    {
        settings ??= new Settings();
        var mode = GameModes.Modes[modeKey];
        var gamePlayers = new List<GamePlayer>();

        for (int i = 0; i < playerIds.Count; i++)
        {
            var id = playerIds[i];
            var src = players.First(p => p.Id == id);
            var gp = new GamePlayer
            {
                Id = id,
                Name = src.Name,
                Color = src.Color,
                Score = mode.Start,
                LegsWon = 0,
                Visits = new(),
                Idx = i,
                DartsThrown = 0,
                Done = false,
                Team = teamMode && teamAssignment != null && i < teamAssignment.Length ? teamAssignment[i] : null
            };

            if (mode.Killer)
            {
                gp.Lives = 3;
                gp.Eliminated = false;
                gp.KillerNumber = GameModes.KillerNumbers[i % GameModes.KillerNumbers.Length];
                gp.KillerHits = 0;
                gp.Kills = new();
            }

            if (mode.Party && modeKey == "battle")
            {
                var attrs = src.Attributes ?? DefaultAttributes(settings);
                var hp = Math.Min(settings.PowerUpScaling.HealthMax, Math.Max(1, attrs.Health));
                gp.Hp = hp;
                gp.MaxHp = hp;
                gp.ArmorPct = Math.Min(settings.PowerUpScaling.ArmorMax, attrs.Armor);
                gp.PowerPct = Math.Min(settings.PowerUpScaling.PowerMax, attrs.Power);
                gp.DamageDealt = 0;
                gp.Defeated = false;
            }

            if (powerUpsEnabled)
            {
                gp.PowerUpCharge = PowerUps.StartingCharge(src.PowerUps?.Active, settings);
                gp.PowerUpUsed = false;
                gp.PowerUpUses = 0;
                gp.PowerUpId = src.PowerUps?.Active;
            }

            gamePlayers.Add(gp);
        }

        int teamCount = 0;
        if (teamMode && teamAssignment != null)
            teamCount = teamAssignment.Distinct().Count();

        return new Game
        {
            Mode = modeKey,
            Players = gamePlayers,
            CurrentIdx = 0,
            CurrentLeg = 1,
            LegsBestOf = legsBestOf,
            DoubleOut = doubleOut,
            Finished = false,
            Practice = mode.Practice,
            Atc = mode.Atc,
            Killer = mode.Killer,
            Party = mode.Party,
            TeamMode = teamMode,
            TeamCount = teamCount,
            PowerUpsEnabled = powerUpsEnabled,
            Date = DateTime.UtcNow,
            ThrownThisRound = new(),
            StartScore = mode.Start,
            Darts = new()
        };
    }

    public static (int xp, int level) LevelFromXp(int totalXp, Settings settings)
    {
        var cfg = settings.XpConfig;
        int level = 1;
        int xp = totalXp;
        int needed = (int)(cfg.BaseLevelXp * cfg.LevelMult);
        while (xp >= needed && level < 999)
        {
            xp -= needed;
            level++;
            needed = (int)(cfg.BaseLevelXp * Math.Pow(cfg.LevelMult, level - 1));
        }
        return (xp, level);
    }

    public static int XpForLevel(int level, Settings settings)
    {
        var cfg = settings.XpConfig;
        return (int)(cfg.BaseLevelXp * Math.Pow(cfg.LevelMult, level - 1));
    }

    public static double VisitAvgStatic(GamePlayer p)
    {
        if (p.Visits == null || p.Visits.Count == 0) return 0;
        var valid = p.Visits.Where(v => !v.Bust).ToList();
        if (valid.Count == 0) return 0;
        return (double)valid.Sum(v => v.Scored) / valid.Count;
    }

    public static double VisitAvgFirst9(GamePlayer p)
    {
        if (p.Visits == null || p.Visits.Count == 0) return 0;
        var first3 = p.Visits.Where(v => !v.Bust).Take(3).ToList();
        if (first3.Count == 0) return 0;
        return (double)first3.Sum(v => v.Scored) / first3.Count;
    }

    public static List<Visit> LifetimeVisits(string playerId, List<GameRecord> games)
    {
        var visits = new List<Visit>();
        foreach (var g in games)
        {
            var gp = g.Players.FirstOrDefault(p => p.Id == playerId);
            if (gp != null) visits.AddRange(gp.Visits.Where(v => !v.Bust && !v.Atc));
        }
        return visits;
    }

    public static PlayerStats PlayerStats_(string playerId, List<GameRecord> games)
    {
        var stats = new PlayerStats();
        var playerGames = games.Where(g => g.Players.Any(p => p.Id == playerId)).ToList();
        stats.Games = playerGames.Count;
        var competitiveGames = playerGames.Where(g => !g.Practice && !g.Party).ToList();
        stats.CompetitiveGames = competitiveGames.Count;
        stats.GamesWon = competitiveGames.Count(g => g.Winner == playerId);
        stats.WinRate = stats.CompetitiveGames > 0 ? (double)stats.GamesWon / stats.CompetitiveGames * 100 : 0;
        var allVisits = new List<Visit>();
        foreach (var g in playerGames)
        {
            var gp = g.Players.FirstOrDefault(p => p.Id == playerId);
            if (gp == null) continue;
            allVisits.AddRange(gp.Visits.Where(v => !v.Bust));
        }
        if (allVisits.Count > 0)
        {
            stats.Avg = (double)allVisits.Sum(v => v.Scored) / allVisits.Count;
            var first9 = allVisits.Take(3);
            stats.First9 = first9.Sum(v => v.Scored) / Math.Min(3, allVisits.Count);
            stats.N180 = allVisits.Count(v => v.Scored == 180);
            stats.N140 = allVisits.Count(v => v.Scored >= 140);
            stats.Tons = allVisits.Count(v => v.Scored >= 100);
            stats.HighScore = allVisits.Max(v => v.Scored);
            stats.HighCheckout = allVisits.Where(v => v.Remaining == 0).Select(v => v.Checkout).DefaultIfEmpty(0).Max();
            stats.LegsWon = playerGames.Sum(g => g.Players.FirstOrDefault(p => p.Id == playerId)?.LegsWon ?? 0);
            stats.DartsThrown = playerGames.Sum(g => g.Players.FirstOrDefault(p => p.Id == playerId)?.DartsThrown ?? 0);
        }
        stats.Kills = playerGames.Where(g => g.Mode == "killer").Sum(g => g.Players.FirstOrDefault(p => p.Id == playerId)?.Kills?.Count ?? 0);
        stats.BattleGames = playerGames.Count(g => g.Mode == "battle");
        return stats;
    }

    public static PlayerAttributes DefaultAttributes(Settings settings) => new()
    {
        Health = settings.PowerUpScaling.AttributeStartHealth,
        Armor = settings.PowerUpScaling.AttributeStartArmor,
        Power = settings.PowerUpScaling.AttributeStartPower,
        PointsAvailable = 0
    };

    public static PlayerPowerUps DefaultPowerUps(Settings settings) => new()
    {
        Unlocked = new(),
        Active = null,
        PointsAvailable = settings.PowerUpScaling.StartingPoints
    };

    public static string Initials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", parts.Select(w => w[0]).Take(2)).ToUpper();
    }

    public static string TodayKey() => DateTime.UtcNow.ToString("yyyy-MM-dd");
    public static string FmtDate(DateTime d) => d.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    public static string FmtTime(DateTime d) => d.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture);
    public static string FmtDateTime(DateTime d) => d.ToString("dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
    public static string FmtDateLong(DateTime d) => d.ToString("ddd, dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

    // ── Power-up charge ──
    public static int ChargeForDart(Dart dart, Settings settings)
    {
        var s = settings.PowerUpScaling;
        if (dart.IsTriple) return (int)s.ChargePerTriple;
        if (dart.IsDouble && !dart.IsBull) return (int)s.ChargePerDouble;
        if (dart.IsBull) return (int)s.ChargePerBull;
        return (int)(dart.Value * s.ChargePerScorePoint);
    }

    public static void AddCharge(GamePlayer gp, int charge, Settings settings)
    {
        if (gp.PowerUpCharge == null) return;
        gp.PowerUpCharge = Math.Min(settings.PowerUpScaling.ChargeMax, gp.PowerUpCharge.Value + charge);
    }

    public static bool IsCharged(GamePlayer gp, Settings settings)
    {
        if (gp.PowerUpCharge == null || gp.PowerUpId == null) return false;
        return gp.PowerUpCharge.Value >= PowerUps.ChargesNeeded(gp.PowerUpId, settings);
    }

    // ── Power-up application ──
    public static PowerUpResult ApplyPowerUp(Game game, int curIdx, Settings settings)
    {
        var gp = game.Players[curIdx];
        var id = gp.PowerUpId;
        if (id == null) return new PowerUpResult { Game = game, Message = "No power-up equipped.", Ok = false };
        if (!IsCharged(gp, settings)) return new PowerUpResult { Game = game, Message = "Power-up not charged yet.", Ok = false };
        if (gp.PowerUpUsed == true) return new PowerUpResult { Game = game, Message = "Power-up already used this game.", Ok = false };

        var result = ApplyPowerUpEffect(game, curIdx, id);
        if (!result.Ok) return result;

        gp.PowerUpUsed = true;
        gp.PowerUpUses = (gp.PowerUpUses ?? 0) + 1;
        gp.PowerUpCharge = 0;
        return result;
    }

    private static PowerUpResult ApplyPowerUpEffect(Game game, int curIdx, string id)
    {
        var players = game.Players;
        switch (id)
        {
            case "pu_fourth_dart":
                return new PowerUpResult { Game = game, Message = "Fourth Dart active — score your bonus dart!" };

            case "pu_blocker":
                {
                    bool anyShielded = false;
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (i == curIdx) continue;
                        if (players[i]._shieldTurns > 0) { players[i]._shieldTurns = 0; anyShielded = true; }
                        else players[i]._oneDartNext = true;
                    }
                    return new PowerUpResult { Game = game, Message = anyShielded ? "Blocker — a Shield absorbed the hit for that player!" : "Blocker! Opponents only get one dart next visit.", ShieldBlocked = anyShielded };
                }

            case "pu_reroll":
                {
                    var darts = game.Darts ?? new();
                    if (darts.Count == 0) return new PowerUpResult { Game = game, Message = "Reroll: no darts to reroll yet.", Ok = false };
                    int idx = 0;
                    for (int i = 1; i < darts.Count; i++) if (darts[i].Value < darts[idx].Value) idx = i;
                    var oldLabel = darts[idx].Label;
                    var me = players[curIdx];
                    double behindFactor = 0;
                    if (players.Count > 1)
                    {
                        if (game.Mode == "highscore")
                        {
                            var leader = players.Max(p => p.Score);
                            behindFactor = Math.Max(0, Math.Min(1, (leader - me.Score) / 200.0));
                        }
                        else
                        {
                            var leader = players.Min(p => p.Score);
                            behindFactor = Math.Max(0, Math.Min(1, (me.Score - leader) / 200.0));
                        }
                    }
                    var newDart = RollRerollDart(behindFactor, me.Score, darts.Where((_, i) => i != idx).Sum(d => d.Value), game.DoubleOut);
                    darts[idx] = newDart;
                    game.Darts = darts;
                    return new PowerUpResult { Game = game, Message = $"Reroll! Replaced {oldLabel} with {newDart.Label} ({newDart.Value})." };
                }

            case "pu_rethrow":
                {
                    var darts = game.Darts ?? new();
                    if (darts.Count == 0) return new PowerUpResult { Game = game, Message = "Re-Throw: no dart to take back yet.", Ok = false };
                    darts.RemoveAt(darts.Count - 1);
                    game.Darts = darts;
                    return new PowerUpResult { Game = game, Message = "Re-Throw! Last dart taken back — throw it again." };
                }

            case "pu_surge":
                {
                    players[curIdx]._surgeNext = true;
                    players[curIdx]._surgeArmed = true;
                    return new PowerUpResult { Game = game, Message = "Surge armed! Your next visit scores double." };
                }

            case "pu_cripple":
                {
                    if (players.Count < 2) return new PowerUpResult { Game = game, Message = "Cripple: no opponents to cripple.", Ok = false };
                    int target = FindLeader(players, curIdx, game.Mode == "highscore");
                    if (players[target]._shieldTurns > 0)
                    {
                        players[target]._shieldTurns = 0;
                        return new PowerUpResult { Game = game, Message = $"Cripple absorbed by {players[target].Name}'s Shield!", ShieldBlocked = true };
                    }
                    players[target]._crippledNext = true;
                    return new PowerUpResult { Game = game, Message = $"Cripple! {players[target].Name} scores 50% next visit." };
                }

            case "pu_steal":
                {
                    if (players.Count < 2) return new PowerUpResult { Game = game, Message = "Steal: no opponents to steal from.", Ok = false };
                    int target = FindLeader(players, curIdx, game.Mode == "highscore");
                    if (players[target]._shieldTurns > 0)
                    {
                        players[target]._shieldTurns = 0;
                        return new PowerUpResult { Game = game, Message = $"Steal absorbed by {players[target].Name}'s Shield!", ShieldBlocked = true };
                    }
                    const int STEAL = 30;
                    if (game.Mode == "highscore")
                    {
                        players[curIdx].Score += STEAL;
                        players[target].Score = Math.Max(0, players[target].Score - STEAL);
                    }
                    else
                    {
                        players[curIdx].Score = Math.Max(0, players[curIdx].Score - STEAL);
                        players[target].Score += STEAL;
                    }
                    return new PowerUpResult { Game = game, Message = $"Steal! Took {STEAL} from {players[target].Name}." };
                }

            case "pu_freeze":
                {
                    bool anyShielded = false;
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (i == curIdx) continue;
                        if (players[i]._shieldTurns > 0) { players[i]._shieldTurns = 0; anyShielded = true; }
                        else players[i]._frozenNext = true;
                    }
                    return new PowerUpResult { Game = game, Message = anyShielded ? "Freeze — a Shield absorbed the hit for that player!" : "Freeze! The leader misses their next visit.", ShieldBlocked = anyShielded };
                }

            case "pu_lucky_miss":
                players[curIdx]._luckyMiss = true;
                return new PowerUpResult { Game = game, Message = "Lucky Miss armed — your next bust is cancelled." };

            case "pu_bullseye_frenzy":
                players[curIdx]._bullseyeFrenzy = true;
                return new PowerUpResult { Game = game, Message = "Bullseye Frenzy armed — next visit, bulls score double!" };

            case "pu_hot_streak":
                players[curIdx]._hotStreak = true;
                return new PowerUpResult { Game = game, Message = "Hot Streak armed — next visit, bonus grows with each dart!" };

            case "pu_swap":
                {
                    if (players.Count < 2) return new PowerUpResult { Game = game, Message = "Swap: no opponents to swap with.", Ok = false };
                    int target = FindLeader(players, curIdx, game.Mode == "highscore");
                    var me = players[curIdx];
                    bool behind = game.Mode == "highscore" ? players[target].Score > me.Score : players[target].Score < me.Score;
                    if (!behind) return new PowerUpResult { Game = game, Message = "Swap: you are already leading — nothing to swap.", Ok = false };
                    if (players[target]._shieldTurns > 0)
                    {
                        players[target]._shieldTurns = 0;
                        return new PowerUpResult { Game = game, Message = $"Swap absorbed by {players[target].Name}'s Shield!", ShieldBlocked = true };
                    }
                    (players[curIdx].Score, players[target].Score) = (players[target].Score, players[curIdx].Score);
                    return new PowerUpResult { Game = game, Message = $"Swap! You traded scores with {players[target].Name}." };
                }

            case "pu_shield":
                players[curIdx]._shieldTurns = 2;
                return new PowerUpResult { Game = game, Message = "Shield active! Protected from power-up attacks for 2 turns." };

            default:
                return new PowerUpResult { Game = game, Message = "Unknown power-up.", Ok = false };
        }
    }

    private static int FindLeader(List<GamePlayer> players, int curIdx, bool highScore)
    {
        int target = -1;
        for (int i = 0; i < players.Count; i++)
        {
            if (i == curIdx) continue;
            if (target == -1) { target = i; continue; }
            if (highScore ? players[i].Score > players[target].Score : players[i].Score < players[target].Score)
                target = i;
        }
        return target;
    }

    private static Dart RollRerollDart(double behindFactor, int remainingBefore, int otherDartsValue, bool doubleOut)
    {
        var rnd = new Random();
        for (int attempt = 0; attempt < 24; attempt++)
        {
            int baseVal = rnd.Next(1, 21);
            double r = rnd.NextDouble();
            double tripleChance = 0.10 + 0.35 * behindFactor;
            double doubleChance = 0.20 + 0.25 * behindFactor;
            int mult = r < tripleChance ? 3 : r < tripleChance + doubleChance ? 2 : 1;
            int value = baseVal * mult;
            string label = (mult == 2 ? "D" : mult == 3 ? "T" : "") + baseVal;
            bool isDouble = mult == 2;
            int newRemaining = remainingBefore - (otherDartsValue + value);
            if (newRemaining == 0)
            {
                if (doubleOut && !isDouble) continue;
                continue;
            }
            return new Dart { Value = value, Label = label, Base = baseVal, Mult = mult, IsDouble = isDouble };
        }
        int maxSafe = Math.Max(1, remainingBefore - otherDartsValue - 1);
        int fb = Math.Max(1, Math.Min(20, maxSafe));
        return new Dart { Value = fb, Label = fb.ToString(), Base = fb, Mult = 1 };
    }

    // ── Battle damage ──
    public static int ComputeBattleDamage(Dart dart, GamePlayer attacker, GamePlayer target, Settings settings)
    {
        if (dart.IsMiss) return 0;
        int power = attacker.PowerPct ?? 0;
        int armor = target.ArmorPct ?? 0;
        int dmg = (dart.Value + power) - armor;
        return Math.Max(settings.PowerUpScaling.BattleMinDamage, dmg);
    }

    // ── XP awarding ──
    public static int AwardXp(Player winner, Game game, GamePlayer gp, Settings settings)
    {
        int xp = 0;
        var cfg = settings.XpConfig;
        foreach (var v in gp.Visits.Where(v => !v.Bust && !v.Atc))
        {
            if (v.Scored >= 180) xp += cfg.Visit180;
            else if (v.Scored >= 140) xp += cfg.Visit140;
            else if (v.Scored >= 120) xp += cfg.Visit120;
            else if (v.Scored >= 100) xp += cfg.Visit100;
            else if (v.Scored >= 80) xp += cfg.Visit80;
            else if (v.Scored >= 60) xp += cfg.Visit60;
            xp += v.Darts.Count * cfg.PerDart;
            if (v.Remaining == 0) xp += cfg.Checkout;
        }
        if (game.Winner == winner.Id && !game.Practice) xp += cfg.Win;
        winner.Xp += xp;
        var (_, level) = LevelFromXp(winner.Xp, settings);
        winner.Level = level;
        return xp;
    }

    // ── Title checking ──
    public static List<string> CheckTitles(Player player, Game game, List<GameRecord> allGames, Settings settings)
    {
        var unlocked = new List<string>();
        var gameVisits = game.Players.FirstOrDefault(p => p.Id == player.Id)?.Visits ?? new();
        var lifetimeVisits = LifetimeVisits(player.Id, allGames);
        var gamesPlayed = allGames.Count(g => g.Players.Any(p => p.Id == player.Id));
        var gamesWon = allGames.Count(g => g.Winner == player.Id);

        foreach (var title in Titles.Builtin)
        {
            if (player.UnlockedTitles.Contains(title.Id)) continue;
            if (CheckTitle(title.Id, game, gameVisits, lifetimeVisits, gamesPlayed, gamesWon, player.Id, allGames))
            {
                unlocked.Add(title.Id);
                player.UnlockedTitles.Add(title.Id);
            }
        }
        return unlocked;
    }

    private static bool CheckTitle(string id, Game game, List<Visit> gameVisits, List<Visit> lifetimeVisits, int gamesPlayed, int gamesWon, string playerId, List<GameRecord> allGames)
    {
        var darts = gameVisits.SelectMany(v => v.Darts ?? new()).ToList();
        var lifeDarts = lifetimeVisits.SelectMany(v => v.Darts ?? new()).ToList();

        return id switch
        {
            "t20king" => darts.Count(d => d.Base == 20 && d.Mult == 3) >= 5,
            "t20trio" => darts.Count(d => d.Base == 20 && d.Mult == 3) >= 3,
            "t20marathon" => darts.Count(d => d.Base == 20 && d.Mult == 3) >= 10,
            "t1master" => darts.Count(d => d.Base == 1 && d.Mult == 3) >= 3,
            "cow_tipper" => darts.Any(d => d.Value == 25),
            "milkman" => darts.Count(d => d.Value == 25) >= 3,
            "cattle_rustler" => darts.Count(d => d.Value == 25 || d.Value == 50) >= 5,
            "first_bull" => darts.Any(d => d.Value == 50),
            "bullseye" => darts.Count(d => d.Value == 50) >= 3,
            "bullseye_pro" => darts.Count(d => d.Value == 50) >= 5,
            "ton80" => gameVisits.Any(v => !v.Atc && v.Scored == 180),
            "double_vision" => gameVisits.Count(v => !v.Atc && v.Scored == 180) >= 2,
            "ton_machine" => gameVisits.Count(v => !v.Atc && v.Scored >= 100) >= 3,
            "ton_factory" => gameVisits.Count(v => !v.Atc && v.Scored >= 100) >= 6,
            "big_hitter" => gameVisits.Count(v => !v.Atc && v.Scored >= 140) >= 3,
            "consistent" => CheckConsecutive(gameVisits, 5, 60),
            "checkout_1" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout > 0 && v.Checkout < 10),
            "checkout_10" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 10),
            "checkout_25" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 25),
            "checkout_40" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 40),
            "checkout_60" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 60),
            "checkout_80" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 80),
            "checkout_king" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 100),
            "big_finish" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 120),
            "high_ton_out" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 150),
            "checkout_170" => gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 170),
            "sharpshooter" => darts.Count(d => d.Mult == 3) >= 10,
            "triple_stack" => gameVisits.Any(v => (v.Darts ?? new()).Count(d => d.Mult == 3) == 3),
            "double_dip" => gameVisits.Any(v => (v.Darts ?? new()).Count(d => d.Mult == 2 && d.Value != 50) == 3),
            "double_trouble" => gameVisits.Count(v => v.Bust) >= 3,
            "comeback_kid" => game.Players.Count > 1 && game.Winner == playerId && game.LegsBestOf > 1,
            "first9_flyer" => CheckFirst9(gameVisits),
            // Lifetime
            "life_score_1k" => lifetimeVisits.Sum(v => v.Scored) >= 1000,
            "life_score_5k" => lifetimeVisits.Sum(v => v.Scored) >= 5000,
            "life_score_10k" => lifetimeVisits.Sum(v => v.Scored) >= 10000,
            "life_score_25k" => lifetimeVisits.Sum(v => v.Scored) >= 25000,
            "life_score_50k" => lifetimeVisits.Sum(v => v.Scored) >= 50000,
            "life_score_100k" => lifetimeVisits.Sum(v => v.Scored) >= 100000,
            "life_t20_25" => lifeDarts.Count(d => d.Base == 20 && d.Mult == 3) >= 25,
            "life_t20_100" => lifeDarts.Count(d => d.Base == 20 && d.Mult == 3) >= 100,
            "life_t20_500" => lifeDarts.Count(d => d.Base == 20 && d.Mult == 3) >= 500,
            "life_180_1" => lifetimeVisits.Count(v => v.Scored == 180) >= 1,
            "life_180_10" => lifetimeVisits.Count(v => v.Scored == 180) >= 10,
            "life_180_25" => lifetimeVisits.Count(v => v.Scored == 180) >= 25,
            "life_180_50" => lifetimeVisits.Count(v => v.Scored == 180) >= 50,
            "life_bulls_10" => lifeDarts.Count(d => d.Value == 50) >= 10,
            "life_bulls_50" => lifeDarts.Count(d => d.Value == 50) >= 50,
            "life_bulls_75" => lifeDarts.Count(d => d.Value == 50) >= 75,
            "life_bulls_100" => lifeDarts.Count(d => d.Value == 50) >= 100,
            "life_triples_50" => lifeDarts.Count(d => d.Mult == 3) >= 50,
            "life_triples_500" => lifeDarts.Count(d => d.Mult == 3) >= 500,
            "life_doubles_50" => lifeDarts.Count(d => d.Mult == 2 && d.Value != 50) >= 50,
            "life_doubles_500" => lifeDarts.Count(d => d.Mult == 2 && d.Value != 50) >= 500,
            "life_checkouts_10" => lifetimeVisits.Count(v => v.Remaining == 0) >= 10,
            "life_checkouts_50" => lifetimeVisits.Count(v => v.Remaining == 0) >= 50,
            "life_checkouts_100" => lifetimeVisits.Count(v => v.Remaining == 0) >= 100,
            "life_checkouts_500" => lifetimeVisits.Count(v => v.Remaining == 0) >= 500,
            "life_games_10" => gamesPlayed >= 10,
            "life_games_50" => gamesPlayed >= 50,
            "life_games_100" => gamesPlayed >= 100,
            "life_games_250" => gamesPlayed >= 250,
            "life_games_500" => gamesPlayed >= 500,
            "life_wins_1" => gamesWon >= 1,
            "life_wins_5" => gamesWon >= 5,
            "life_wins_10" => gamesWon >= 10,
            "life_wins_25" => gamesWon >= 25,
            "life_wins_50" => gamesWon >= 50,
            "life_wins_100" => gamesWon >= 100,
            // Killer
            "killer_first" => game.Mode == "killer" && game.Players.Any(p => (p.Kills?.Count ?? 0) >= 1),
            "killer_3" => game.Mode == "killer" && game.Players.Any(p => (p.Kills?.Count ?? 0) >= 3),
            "killer_survivor" => game.Mode == "killer" && game.Players.Count >= 2 && game.Winner == playerId,
            "killer_marked" => game.Mode == "killer" && game.Players.Any(p => (p.KillerHits ?? 0) >= 5),
            "killer_flawless" => game.Mode == "killer" && game.Players.Count >= 2 && game.Winner == playerId && game.Players.First(p => p.Id == playerId).Lives >= 3,
            // Party
            "speed_demon" => game.Mode == "speed101" && game.Players.Count >= 2 && game.Winner == playerId,
            "speed_blur" => gameVisits.Any(v => v.Remaining == 0 && (v.Darts?.Count ?? 0) <= 3),
            "high_roller" => game.Mode == "highscore" && game.Players.Count >= 2 && game.Winner == playerId,
            "jackpot" => game.Mode == "highscore" && gameVisits.Where(v => !v.Atc).Sum(v => v.Scored) >= 400,
            "party_animal" => allGames.Count(g => g.Mode == "speed101" || g.Mode == "highscore") >= 5,
            // Battle
            "battle_first_win" => game.Mode == "battle" && game.Players.Count >= 2 && game.Winner == playerId,
            "battle_5_wins" => allGames.Count(g => g.Mode == "battle" && g.Winner == playerId) >= 5,
            "battle_25_wins" => allGames.Count(g => g.Mode == "battle" && g.Winner == playerId) >= 25,
            "battle_titan" => allGames.Count(g => g.Mode == "battle" && g.Winner == playerId) >= 100,
            "battle_knockout" => game.Mode == "battle" && game.Players.Any(p => p.Defeated == true || (p.Hp ?? 0) <= 0),
            "battle_flawless" => game.Mode == "battle" && game.Players.Count >= 2 && game.Winner == playerId && (game.Players.First(p => p.Id == playerId).Hp ?? 0) >= (game.Players.First(p => p.Id == playerId).MaxHp ?? 1) * 0.5,
            "battle_3v1" => game.Mode == "battle" && game.Players.Count >= 4 && game.Winner == playerId,
            "battle_bruiser" => game.Mode == "battle" && game.Players.Any(p => (p.DamageDealt ?? 0) >= 500),
            "battle_ironhide" => game.Mode == "battle" && game.Players.Any(p => (p.ArmorPct ?? 0) >= 25),
            "battle_powerhouse" => game.Mode == "battle" && game.Players.Any(p => (p.PowerPct ?? 0) >= 30),
            // Team
            "team_first_win" => game.TeamMode && game.WinningTeam != null && game.Players.Any(p => p.Id == playerId && p.Team == game.WinningTeam),
            "team_5_wins" => allGames.Count(g => g.TeamMode && g.WinningTeam != null && g.Players.Any(p => p.Id == playerId && p.Team == g.WinningTeam)) >= 5,
            "team_25_wins" => allGames.Count(g => g.TeamMode && g.WinningTeam != null && g.Players.Any(p => p.Id == playerId && p.Team == g.WinningTeam)) >= 25,
            "team_4stack" => game.TeamMode && game.TeamCount == 4 && game.WinningTeam != null,
            "team_clutch" => game.TeamMode && game.LegsBestOf > 1 && game.WinningTeam != null,
            "team_rivalry" => game.TeamMode && game.TeamCount >= 2,
            _ => false,
        };
    }

    private static bool CheckConsecutive(List<Visit> visits, int count, int minScore)
    {
        var valid = visits.Where(v => !v.Bust && !v.Atc).ToList();
        for (int i = 0; i <= valid.Count - count; i++)
            if (valid.Skip(i).Take(count).All(v => v.Scored >= minScore)) return true;
        return false;
    }

    private static bool CheckFirst9(List<Visit> visits)
    {
        var byLeg = visits.Where(v => !v.Bust && !v.Atc).GroupBy(v => v.Leg).ToDictionary(g => g.Key, g => g.ToList());
        foreach (var arr in byLeg.Values)
            if (arr.Take(3).Sum(v => v.Scored) >= 120) return true;
        return false;
    }

    // ── Badge checking ──
    public static List<string> CheckBadges(Player player, Game game, List<GameRecord> allGames)
    {
        var unlocked = new List<string>();
        var gp = game.Players.FirstOrDefault(p => p.Id == player.Id);
        if (gp == null) return unlocked;
        var gameVisits = gp.Visits;
        var gamesPlayed = allGames.Count(g => g.Players.Any(p => p.Id == player.Id));
        var gamesWon = allGames.Count(g => g.Winner == player.Id);

        void Check(string id, bool cond)
        {
            if (!player.UnlockedBadges.Contains(id) && cond)
            {
                unlocked.Add(id);
                player.UnlockedBadges.Add(id);
                player.BadgeCounts ??= new();
                player.BadgeCounts[id] = (player.BadgeCounts.GetValueOrDefault(id)) + 1;
            }
        }

        Check("b_first_game", gamesPlayed >= 1);
        Check("b_first_win", gamesWon >= 1);
        Check("b_180", gameVisits.Any(v => v.Scored == 180));
        Check("b_checkout_100", gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 100));
        Check("b_checkout_170", gameVisits.Any(v => v.Remaining == 0 && v.Checkout >= 170));
        Check("b_10_games", gamesPlayed >= 10);
        Check("b_50_games", gamesPlayed >= 50);
        Check("b_100_games", gamesPlayed >= 100);
        Check("b_5_wins", gamesWon >= 5);
        Check("b_25_wins", gamesWon >= 25);
        Check("b_killer", game.Mode == "killer" && game.Winner == player.Id);
        Check("b_battle", game.Mode == "battle" && game.Winner == player.Id);
        Check("b_highscore", game.Mode == "highscore" && game.Winner == player.Id);
        Check("b_team", game.TeamMode && game.WinningTeam != null && gp.Team == game.WinningTeam);
        Check("b_t20_10", gameVisits.SelectMany(v => v.Darts ?? new()).Count(d => d.Base == 20 && d.Mult == 3) >= 10);
        Check("b_bull_5", gameVisits.SelectMany(v => v.Darts ?? new()).Count(d => d.Value == 50) >= 5);
        Check("b_comeback", game.LegsBestOf > 1 && game.Winner == player.Id);
        return unlocked;
    }
}

public class PlayerStats
{
    public int Games { get; set; }
    public int CompetitiveGames { get; set; }
    public int GamesWon { get; set; }
    public double WinRate { get; set; }
    public double Avg { get; set; }
    public double First9 { get; set; }
    public int N180 { get; set; }
    public int N140 { get; set; }
    public int Tons { get; set; }
    public int HighScore { get; set; }
    public int HighCheckout { get; set; }
    public int LegsWon { get; set; }
    public int DartsThrown { get; set; }
    public int Kills { get; set; }
    public int Defeated { get; set; }
    public int BattleGames { get; set; }
}
