namespace dart_counter.Logic;

using dart_counter.Models;

public static class CoopParty
{
    static int InstanceCounter;

    public static string NextInstanceId(string defId) => $"{defId}_{++InstanceCounter}";

    public static CoopPlayer ToCoopPlayer(Player p, Settings settings, int startCharge)
    {
        var cfg = settings.PowerUpScaling;
        var h = p.Attributes?.Health ?? cfg.AttributeStartHealth;
        var a = p.Attributes?.Armor ?? cfg.AttributeStartArmor;
        var pw = p.Attributes?.Power ?? cfg.AttributeStartPower;
        return new()
        {
            Id = p.Id,
            Name = p.Name,
            Color = p.Color,
            Hp = Math.Max(1, Math.Min(cfg.HealthMax, h)),
            MaxHp = Math.Max(1, Math.Min(cfg.HealthMax, h)),
            Power = Math.Max(0, Math.Min(cfg.PowerMax, pw)),
            Armor = Math.Max(0, Math.Min(cfg.ArmorMax, a)),
            Buffs = new(),
            PowerUpCharge = Math.Max(0, startCharge),
            ClassId = p.CoopProgress?.ClassId,
            Kills = 0,
            DamageDealt = 0
        };
    }

    public static int PartyMaxHpFor(List<Player> players, Settings settings)
    {
        var cfg = settings.PowerUpScaling;
        if (players.Count == 0) return 1;
        var sum = players.Sum(p =>
        {
            var h = p.Attributes?.Health;
            return h.HasValue ? Math.Max(1, h.Value) : cfg.AttributeStartHealth;
        });
        var avg = (double)sum / players.Count;
        return Math.Max(1, Math.Min(cfg.HealthMax, (int)Math.Round(avg)));
    }
}
