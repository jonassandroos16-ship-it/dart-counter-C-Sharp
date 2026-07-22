using System.Text.Json.Serialization;

namespace dart_counter.Models;

public enum GameModeType { Dartboard, Cards }

public enum PlayerSoundId { None, Hero, Villain, Cyborg, Mystic, Beast, Champion }

public class PlayerAttributes
{
    public int Health { get; set; } = 300;
    public int Armor { get; set; } = 0;
    public int Power { get; set; } = 0;
    public int PointsAvailable { get; set; } = 0;
}

public class PlayerPowerUps
{
    public List<string> Unlocked { get; set; } = new();
    public string? Active { get; set; }
    public int PointsAvailable { get; set; } = 0;
    public List<string>? CoopUnlocked { get; set; }
    public string? CoopActive { get; set; }
}

public class Player
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#22c55e";
    public int Xp { get; set; }
    public int Level { get; set; } = 1;
    public List<string> UnlockedTitles { get; set; } = new();
    public string? SelectedTitle { get; set; }
    public List<string> UnlockedBadges { get; set; } = new();
    public Dictionary<string, int>? BadgeCounts { get; set; }
    public string? SelectedBadge { get; set; }
    public bool ShowBadgeContext { get; set; }
    public PlayerSoundId Sound { get; set; } = PlayerSoundId.None;
    public PlayerAttributes? Attributes { get; set; }
    public PlayerPowerUps? PowerUps { get; set; }
    public bool DeveloperMode { get; set; }
    public string? ShowdownBg { get; set; }
    public PlayerCoopProgress? CoopProgress { get; set; }
    public PlayerCampaignProgress? CampaignProgress { get; set; }
    public PlayerDartliteStats? DartliteStats { get; set; }
    public Dictionary<string, List<PlayerCard>>? Cards { get; set; }
}

public class Dart
{
    public int Value { get; set; }
    public string Label { get; set; } = "";
    public int Base { get; set; }
    public int Mult { get; set; } = 1;
    public bool IsDouble { get; set; }
    public bool IsTriple { get; set; }
    public bool IsBull { get; set; }
    public bool IsOuter { get; set; }
    public bool IsMiss { get; set; }
}

public class GamePlayer
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#22c55e";
    public int Score { get; set; }
    public int StartScore { get; set; }
    public List<Dart> Darts { get; set; } = new();
    public int? LegWins { get; set; }
    public int? SetWins { get; set; }
    public int? Team { get; set; }
    public bool Eliminated { get; set; }
    public bool IsWinner { get; set; }
    public int? Rank { get; set; }
    public int? AvgVisitScore { get; set; }
    public int? VisitCount { get; set; }
    public int? First9Avg { get; set; }
    public int? HighestScore { get; set; }
    public int? HighestRun { get; set; }
    public int? Checkout { get; set; }
    public int? CheckoutAttempts { get; set; }
    public int? Ton80s { get; set; }
    public int? TonPlus { get; set; }
    public int? OneEighties { get; set; }
    public int? One40Plus { get; set; }
    public int? One60Plus { get; set; }
    public int? Bulls { get; set; }
    public int? Bullseyes { get; set; }
    public int? Misses { get; set; }
    public int? Doubles { get; set; }
    public int? Triples { get; set; }
    public int? PowerUpsUsed { get; set; }
    public int? Busts { get; set; }
    public int? Visits { get; set; }
}

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Mode { get; set; } = "501";
    public string ModeLabel { get; set; } = "501";
    public bool DoubleOut { get; set; }
    public int LegsTarget { get; set; } = 1;
    public int SetsTarget { get; set; } = 1;
    public bool TeamMode { get; set; }
    public bool PowerUps { get; set; }
    public List<GamePlayer> Players { get; set; } = new();
    public int CurrentIdx { get; set; }
    public int CurrentLeg { get; set; } = 1;
    public int CurrentSet { get; set; } = 1;
    public bool Finished { get; set; }
    public string? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public string? Note { get; set; }
    public bool Ranked { get; set; }
    public string? ActivePowerUp { get; set; }
    public int? PowerUpTargetIdx { get; set; }
    public string? PowerUpState { get; set; }
    public bool PowerUpPending { get; set; }
    public string? ActiveShield { get; set; }
    public int? ShieldTurns { get; set; }
    public List<string>? Log { get; set; }
    public int? First9Avg { get; set; }
    public int? HighestScore { get; set; }
    public int? HighestRun { get; set; }
    public int? Ton80s { get; set; }
    public int? OneEighties { get; set; }
    public int? Bulls { get; set; }
    public int? Bullseyes { get; set; }
    public int? Busts { get; set; }
    public int? Visits { get; set; }
}

public class GameRecord
{
    public string Id { get; set; } = "";
    public Game Game { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class Settings
{
    public string Theme { get; set; } = "dark";
    public string Accent { get; set; } = "#22c55e";
    public bool Sound { get; set; } = true;
    public string ClickSound { get; set; } = "tick";
    public string HitSound { get; set; } = "thud";
    public string Sfx { get; set; } = "hero";
    public bool Music { get; set; }
    public double MusicVolume { get; set; } = 0.3;
    public string MusicStartTrack { get; set; } = "start_bullseye_anthem";
    public string MusicSetupTrack { get; set; } = "setup_horizon";
    public string MusicMatchTrack { get; set; } = "match_drive";
    public string MusicCoopTrack { get; set; } = "coop_siege";
    public int StartScore { get; set; } = 501;
    public bool DoubleOut { get; set; } = true;
    public int LegsTarget { get; set; } = 3;
    public int SetsTarget { get; set; } = 1;
    public bool TeamMode { get; set; }
    public bool PowerUps { get; set; }
    public GameModeType GameModeType { get; set; } = GameModeType.Dartboard;
    public bool ShowSetup { get; set; } = true;
    public bool ShowFirst9Avg { get; set; } = true;
    public bool ShowHighestScore { get; set; } = true;
    public bool ShowHighestRun { get; set; } = true;
    public bool ShowTon80s { get; set; } = true;
    public bool ShowOneEighties { get; set; } = true;
    public bool ShowBulls { get; set; } = true;
    public bool ShowBullseyes { get; set; } = true;
    public bool ShowBusts { get; set; } = true;
    public bool ShowVisits { get; set; } = true;
    public bool ShowCheckoutPct { get; set; } = true;
    public bool ShowAvgVisitScore { get; set; } = true;
    public bool ShowPowerUpStats { get; set; } = true;
    public List<string> CustomTitles { get; set; } = new();
    public PowerUpScaling PowerUpScaling { get; set; } = new();
    public bool WelcomeShown { get; set; }
}

public class PowerUpScaling
{
    public int AttributeStartHealth { get; set; } = 400;
    public int AttributeStartArmor { get; set; } = 0;
    public int AttributeStartPower { get; set; } = 0;
    public int AttributeMaxHealth { get; set; } = 800;
    public int AttributeMaxArmor { get; set; } = 50;
    public int AttributeMaxPower { get; set; } = 100;
    public int ChargeMax { get; set; } = 100;
    public int ChargePerHit { get; set; } = 12;
    public int ChargePerVisit { get; set; } = 8;
    public int XpPerLevel { get; set; } = 1000;
    public int XpPerKill { get; set; } = 20;
    public int XpPerWin { get; set; } = 50;
    public int XpPerLoss { get; set; } = 15;
    public int XpPerVisit { get; set; } = 5;
    public int XpPerDart { get; set; } = 2;
    public int XpPer180 { get; set; } = 200;
    public int XpPerTon { get; set; } = 50;
    public int XpPerBullseye { get; set; } = 30;
    public int XpPerCheckout { get; set; } = 100;
}
