using System.Text.Json.Serialization;

namespace dart_counter.Models;

public class DartliteRunStats
{
    public int RoundsCleared { get; set; }
    public int EnemiesDefeated { get; set; }
    public int MiniBossesDefeated { get; set; }
    public int BossesDefeated { get; set; }
    public int DamageDealt { get; set; }
    public int XpGained { get; set; }
    public List<string> TrinketsCollected { get; set; } = new();
}

public class DartlitePlayerRunStats
{
    public string PlayerId { get; set; } = "";
    public int Kills { get; set; }
    public int DamageDealt { get; set; }
    public List<ChoiceOption> Rewards { get; set; } = new();
    public List<string> Trinkets { get; set; } = new();
}

public class DartliteRunPlayer
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#22c55e";
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Power { get; set; }
    public int Armor { get; set; }
    public List<string> Trinkets { get; set; } = new();
    public int BonusHealth { get; set; }
    public int BonusArmor { get; set; }
    public int BonusPower { get; set; }
    public List<PlayerCard> Cards { get; set; } = new();
}

public enum DartlitePhase { Setup, Battle, Choice, Reward, BossVictory, GameOver }

public enum ChoiceKind { Heal, Stat, Trinket, CardNew, CardUpgrade, DeckUpgrade }

public class ChoiceOption
{
    public ChoiceKind Kind { get; set; }
    public string Label { get; set; } = "";
    public string Desc { get; set; } = "";
    public string Icon { get; set; } = "";
    public string? Stat { get; set; }
    public int? Amount { get; set; }
    public string? TrinketId { get; set; }
    public string? CardId { get; set; }
    public string? CardName { get; set; }
}

public class BossVictoryState
{
    public string BossName { get; set; } = "";
    public List<string> TrinketOptions { get; set; } = new();
    public string? ChosenTrinket { get; set; }
    public string? ClaimedTrinket { get; set; }
}

public class DartliteRun
{
    public int Round { get; set; }
    public List<string> PlayerIds { get; set; } = new();
    public List<DartliteRunPlayer> RunPlayers { get; set; } = new();
    public List<string> Trinkets { get; set; } = new();
    public List<string> Pool { get; set; } = new();
    public DartliteRunStats Stats { get; set; } = new();
    public List<DartlitePlayerRunStats> PlayerStats { get; set; } = new();
    public DartlitePhase Phase { get; set; } = DartlitePhase.Setup;
    public bool CardMode { get; set; }
    public CampaignBattleState? Battle { get; set; }
    public List<ChoiceOption>? PendingChoice { get; set; }
    public int ChoicePlayerIdx { get; set; }
    public List<ChoiceOption?> PlayerChoices { get; set; } = new();
    public string? LastUnlockedTrinket { get; set; }
    public BossVictoryState? BossVictory { get; set; }
    public List<string> Log { get; set; } = new();
}

public class PlayerDartliteStats
{
    public int Kills { get; set; }
    public int Battles { get; set; }
    public int MiniBossesDefeated { get; set; }
    public int BossesDefeated { get; set; }
    public int BestRound { get; set; }
    public int TotalXp { get; set; }
    public int Runs { get; set; }
    public List<string> SeenTrinkets { get; set; } = new();
}

public class DartliteGlobalStats
{
    public int TotalKills { get; set; }
    public int TotalBattles { get; set; }
    public int TotalMiniBosses { get; set; }
    public int TotalBosses { get; set; }
    public int BestRound { get; set; }
    public int TotalRuns { get; set; }
    public int TotalXp { get; set; }
}
