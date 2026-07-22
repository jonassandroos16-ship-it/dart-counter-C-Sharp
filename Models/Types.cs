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
    public int Level { get; set; }
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
}

public class Dart
{
    public int Value { get; set; }
    public string Label { get; set; } = "";
    public bool IsDouble { get; set; }
    public bool IsTriple { get; set; }
    public bool IsBull { get; set; }
    public bool IsMiss => Value == 0;
}

public class Visit
{
    public List<Dart> Darts { get; set; } = new();
    public int Scored { get; set; }
    public int Remaining { get; set; }
    public bool Bust { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int Hits { get; set; }
    public int EndIdx { get; set; }
}

public class GamePlayer
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#22c55e";
    public int Score { get; set; }
    public int LegsWon { get; set; }
    public List<Visit> Visits { get; set; } = new();
    public int Idx { get; set; }
    public int DartsThrown { get; set; }
    public bool Done { get; set; }
    public int? Team { get; set; }
    public int? Lives { get; set; }
    public bool? Eliminated { get; set; }
    public int? KillerNumber { get; set; }
    public int? KillerHits { get; set; }
    public List<string>? Kills { get; set; }
    public int? PowerUpCharge { get; set; }
    public bool? PowerUpUsed { get; set; }
    public int? PowerUpUses { get; set; }
    public string? PowerUpId { get; set; }
}

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Mode { get; set; } = "501";
    public List<GamePlayer> Players { get; set; } = new();
    public int CurrentIdx { get; set; }
    public int CurrentLeg { get; set; } = 1;
    public int LegsBestOf { get; set; } = 1;
    public bool DoubleOut { get; set; }
    public bool Finished { get; set; }
    public string? Winner { get; set; }
    public bool Tied { get; set; }
    public List<string>? TiedPlayers { get; set; }
    public bool Practice { get; set; }
    public bool Atc { get; set; }
    public bool Killer { get; set; }
    public bool Party { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public List<int>? ThrownThisRound { get; set; }
    public int StartScore { get; set; } = 501;
}

public class GameRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Mode { get; set; } = "501";
    public List<GamePlayer> Players { get; set; } = new();
    public string? Winner { get; set; }
    public bool Tied { get; set; }
    public List<string>? TiedPlayers { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public bool Practice { get; set; }
    public bool Atc { get; set; }
    public bool Party { get; set; }
}

public class XpConfig
{
    public int Win { get; set; } = 50;
    public int Visit60 { get; set; } = 5;
    public int Visit80 { get; set; } = 10;
    public int Visit100 { get; set; } = 15;
    public int Visit120 { get; set; } = 20;
    public int Visit140 { get; set; } = 25;
    public int Visit180 { get; set; } = 50;
    public int Checkout { get; set; } = 10;
    public int PerDart { get; set; } = 1;
    public double LevelMult { get; set; } = 1.5;
    public int BaseLevelXp { get; set; } = 100;
}

public class PowerUpScaling
{
    public double ChargePerDouble { get; set; } = 8;
    public double ChargePerTriple { get; set; } = 12;
    public double ChargePerBull { get; set; } = 15;
    public double ChargePerScorePoint { get; set; } = 0.05;
    public int ChargeMax { get; set; } = 100;
    public int PointsPerLevel { get; set; } = 1;
    public int StartingPoints { get; set; } = 1;
    public int AttributePointsPerLevel { get; set; } = 5;
    public int AttributeStartHealth { get; set; } = 300;
    public int AttributeStartArmor { get; set; } = 0;
    public int AttributeStartPower { get; set; } = 0;
    public int HealthPerPoint { get; set; } = 25;
    public int ArmorPerPoint { get; set; } = 1;
    public int PowerPerPoint { get; set; } = 1;
    public int ArmorMax { get; set; } = 25;
    public int PowerMax { get; set; } = 30;
    public int HealthMax { get; set; } = 1000;
    public int BattleMinDamage { get; set; } = 1;
    public Dictionary<string, int>? StartingCharge { get; set; }
    public Dictionary<string, int>? ChargesNeeded { get; set; }
}

public class PopupSettings
{
    public bool Scores { get; set; } = true;
    public bool Milestones { get; set; } = true;
    public bool Xp { get; set; } = true;
    public bool Titles { get; set; } = true;
}

public class CustomTitle
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Desc { get; set; } = "";
}

public class Settings
{
    public string Theme { get; set; } = "dark";
    public string Accent { get; set; } = "#22c55e";
    public GameModeType GameMode { get; set; } = GameModeType.Dartboard;
    public bool ConfirmReset { get; set; } = true;
    public bool Sound { get; set; } = true;
    public bool Music { get; set; } = true;
    public string MusicStartTrack { get; set; } = "start_bullseye_anthem";
    public string MusicSetupTrack { get; set; } = "setup_horizon";
    public string MusicMatchTrack { get; set; } = "match_drive";
    public string MusicCoopTrack { get; set; } = "coop_siege";
    public double SfxVolume { get; set; } = 0.9;
    public double MusicVolume { get; set; } = 0.9;
    public string HitSoundPack { get; set; } = "thud";
    public string ClickSound { get; set; } = "tick";
    public double ClickVolume { get; set; } = 0.6;
    public XpConfig XpConfig { get; set; } = new();
    public List<CustomTitle> CustomTitles { get; set; } = new();
    public PopupSettings Popups { get; set; } = new();
    public PowerUpScaling PowerUpScaling { get; set; } = new();
}
