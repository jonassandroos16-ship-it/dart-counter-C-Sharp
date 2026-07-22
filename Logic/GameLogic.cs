using dart_counter.Models;

namespace dart_counter.Logic;

public static class GameLogic
{
    public static Game CreateGame(string modeKey, List<string> playerIds, List<Player> players, bool doubleOut, int legsBestOf, bool teamMode = false, int[]? teamAssignment = null, bool powerUpsEnabled = false, Settings? settings = null)
    {
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
                Idx = 0,
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

            if (powerUpsEnabled)
            {
                gp.PowerUpCharge = 0;
                gp.PowerUpUsed = false;
                gp.PowerUpUses = 0;
                gp.PowerUpId = src.PowerUps?.Active;
            }

            gamePlayers.Add(gp);
        }

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
            Date = DateTime.UtcNow,
            ThrownThisRound = new(),
            StartScore = mode.Start
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

    public static int GetPlayerXp(List<Player> players, string id)
    {
        return players.FirstOrDefault(p => p.Id == id)?.Xp ?? 0;
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
            stats.First9 = allVisits.Take(3).Sum(v => v.Scored) / Math.Min(3, allVisits.Count);
            stats.N180 = allVisits.Count(v => v.Scored == 180);
            stats.N140 = allVisits.Count(v => v.Scored >= 140);
            stats.Tons = allVisits.Count(v => v.Scored >= 100);
            stats.HighScore = allVisits.Max(v => v.Scored);
        }

        return stats;
    }

    public static PlayerAttributes DefaultAttributes(Settings settings)
    {
        var scaling = settings.PowerUpScaling;
        return new PlayerAttributes
        {
            Health = scaling.AttributeStartHealth,
            Armor = scaling.AttributeStartArmor,
            Power = scaling.AttributeStartPower,
            PointsAvailable = 0
        };
    }

    public static PlayerPowerUps DefaultPowerUps(Settings settings)
    {
        return new PlayerPowerUps
        {
            Unlocked = new(),
            Active = null,
            PointsAvailable = settings.PowerUpScaling.StartingPoints
        };
    }

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
