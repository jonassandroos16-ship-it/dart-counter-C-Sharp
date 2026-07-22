using System.Text.Json.Serialization;

namespace dart_counter.Models;

public enum CoopPhase { Player, Enemy }
public enum CoopOutcome { Ongoing, Victory, Defeat }
public enum CoopPowerUpTier { Starter, Advanced }
public enum Difficulty { Easy, Hard, Boss }
public enum ShieldType { Span, Exact }
public enum CoopClassId { Warrior, Priest, Rogue }

public enum SpanTarget
{
    TopHalf, BottomHalf, LeftHalf, RightHalf,
    AnyDouble, AnyTriple, AnyBull
}

public class ShieldLayer
{
    public ShieldType Type { get; set; }
    public string TargetValue { get; set; } = "";

    [JsonIgnore] public SpanTarget? Span => Type == ShieldType.Span
        ? Enum.TryParse<SpanTarget>(TargetValue.Replace("_", ""), true, out var s) ? s : null
        : null;
}

public class EnemyDef
{
    public string Name { get; set; } = "";
    public Difficulty Difficulty { get; set; }
    public int MaxHp { get; set; }
    public int Armor { get; set; }
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public List<ShieldLayer> Shields { get; set; } = new();
}

public class CampaignLevel
{
    public int LevelId { get; set; }
    public string Name { get; set; } = "";
    public bool IsBoss { get; set; }
    public List<string> Enemies { get; set; } = new();
    public string? RewardPowerUp { get; set; }
    public string? StoryBit { get; set; }
}

public class ChapterTheme
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Background { get; set; } = "";
    public string Accent { get; set; } = "";
    public string CardTint { get; set; } = "";
}

public class CampaignChapter
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public ChapterTheme Theme { get; set; } = new();
    public string Intro { get; set; } = "";
    public string Outro { get; set; } = "";
    public List<CampaignLevel> Levels { get; set; } = new();
}

public class CoopPowerUpDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Desc { get; set; } = "";
    public int Cost { get; set; }
    public CoopPowerUpTier Tier { get; set; }
}

public class CoopClassDef
{
    public CoopClassId Id { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Desc { get; set; } = "";
    public string StarterPassive { get; set; } = "";
}

public class CoopPassiveDef
{
    public string Id { get; set; } = "";
    public CoopClassId ClassId { get; set; }
    public int Tier { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Desc { get; set; } = "";
    public int Power { get; set; }
    public int Health { get; set; }
    public int Armor { get; set; }
    public int LevelRequired { get; set; }
}

public class PlayerBuff
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "power";
    public int Amount { get; set; }
    public int TurnsLeft { get; set; }
    public string Source { get; set; } = "";
}

public class CoopPlayer
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#22c55e";
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Power { get; set; }
    public int Armor { get; set; }
    public List<PlayerBuff> Buffs { get; set; } = new();
    public int PowerUpCharge { get; set; }
    public CoopClassId? ClassId { get; set; }
    public int Kills { get; set; }
    public int DamageDealt { get; set; }
}

public class CampaignDart
{
    public int Value { get; set; }
    public string Label { get; set; } = "";
    public int Base { get; set; }
    public int Mult { get; set; } = 1;
    public bool IsDouble { get; set; }
    public bool IsBull { get; set; }
}

public class ResolvedDart
{
    public CampaignDart Dart { get; set; } = new();
    public int Damage { get; set; }
    public string Kind { get; set; } = "miss";
    public string? ShieldTarget { get; set; }
    public string EnemyId { get; set; } = "";
    public string EnemyName { get; set; } = "";
    public int HpAfter { get; set; }
    public int AttackerPower { get; set; }
    public int TargetArmor { get; set; }
    public bool Vulnerable { get; set; }
}

public class EnemyAttackStep
{
    public string EnemyId { get; set; } = "";
    public string EnemyName { get; set; } = "";
    public CampaignDart Dart { get; set; } = new();
    public int Damage { get; set; }
    public int PartyHpAfter { get; set; }
}

public class ActiveEnemy
{
    public string Id { get; set; } = "";
    public string DefId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Armor { get; set; }
    public double Accuracy { get; set; }
    public double Precision { get; set; }
    public List<ShieldLayer> Shields { get; set; } = new();
    public bool Defeated { get; set; }
    public int FrozenTurns { get; set; }
    public int VulnerableTurns { get; set; }
    public int DistractedTurns { get; set; }
    public double DistractAmount { get; set; }
}

public class FrozenEnemyInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int FrozenTurns { get; set; }
}

public class PartyPassiveSource
{
    public string PlayerId { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string PassiveName { get; set; } = "";
    public string Icon { get; set; } = "";
    public int Power { get; set; }
    public int Health { get; set; }
    public int Armor { get; set; }
}

public class PartyPassiveBonus
{
    public int Power { get; set; }
    public int Health { get; set; }
    public int Armor { get; set; }
    public List<PartyPassiveSource> Sources { get; set; } = new();
}

public class BattleStats
{
    public int VisitsUsed { get; set; }
    public int DartsThrown { get; set; }
    public int DamageDealt { get; set; }
    public int EnemiesDefeated { get; set; }
    public int PowerUpsUsed { get; set; }
    public int PartyHpLost { get; set; }
}

public class CampaignBattleState
{
    public int LevelId { get; set; }
    public string LevelName { get; set; } = "";
    public bool IsBoss { get; set; }
    public int PartyHp { get; set; }
    public int PartyMaxHp { get; set; }
    public List<CoopPlayer> Players { get; set; } = new();
    public string ChapterId { get; set; } = "crimson_vale";
    public BattleStats Stats { get; set; } = new();
    public int PlayerTurnIdx { get; set; }
    public List<CampaignDart> Darts { get; set; } = new();
    public List<ActiveEnemy> Enemies { get; set; } = new();
    public int TargetIdx { get; set; }
    public CoopPhase Phase { get; set; } = CoopPhase.Player;
    public int VisitNumber { get; set; } = 1;
    public CoopOutcome Outcome { get; set; } = CoopOutcome.Ongoing;
    public int PowerUpCharge { get; set; }
    public List<ResolvedDart> ResolvedDarts { get; set; } = new();
    public List<ActiveEnemy> VisitEnemiesSnapshot { get; set; } = new();
    public List<EnemyAttackStep> PendingEnemyAttacks { get; set; } = new();
    public List<EnemyAttackStep> AppliedEnemyAttacks { get; set; } = new();
    public bool AwaitContinue { get; set; }
    public int PhantomDarts { get; set; }
    public List<FrozenEnemyInfo> FrozenEnemiesThisRound { get; set; } = new();
    public PartyPassiveBonus? PassiveBonus { get; set; }
}

public class PlayerCoopProgress
{
    public CoopClassId? ClassId { get; set; } = CoopClassId.Warrior;
    public List<string> UnlockedPassives { get; set; } = new();
    public List<string> EquippedPassives { get; set; } = new();
}

public class PlayerCampaignProgress
{
    public int HighestLevelBeaten { get; set; }
    public List<string> UnlockedPowerUps { get; set; } = new();
    public Dictionary<string, int> Chapters { get; set; } = new();
}
