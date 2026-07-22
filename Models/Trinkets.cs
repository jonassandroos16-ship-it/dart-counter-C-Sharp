namespace dart_counter.Models;

public enum TrinketTier { Tier1 = 1, Tier2 = 2, Tier3 = 3, Tier4 = 4 }

public class TrinketDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public TrinketTier Tier { get; set; }
    public string Desc { get; set; } = "";
}

public static class Trinkets
{
    public static readonly string[] StarterPool =
    {
        "trk_sharp_tip", "trk_thick_hide", "trk_vitality", "trk_lucky_penny", "trk_quick_reflex",
    };

    public static readonly string[] MinibossPool =
    {
        "trk_double_tap", "trk_vampiric", "trk_bulwark", "trk_eagle_eye", "trk_phantom_step",
    };

    public static readonly string[] BossPool =
    {
        "trk_berserker", "trk_frozen_core", "trk_overcharge", "trk_executioner", "trk_second_wind",
        "trk_giants_belt", "trk_soul_harvest", "trk_chain_lightning", "trk_adamant", "trk_phoenix_heart",
    };

    public static readonly string[] BossTrinketPool1 = { "trk_boss_warlords_crown", "trk_boss_ice_crystal", "trk_boss_verdant_seed" };
    public static readonly string[] BossTrinketPool2 = { "trk_boss_dragon_heart", "trk_boss_frost_throne", "trk_boss_maw_jaw" };
    public static readonly string[] BossTrinketPoolRest = { "trk_boss_void_cloak", "trk_boss_eternal_flame", "trk_boss_titan_heart", "trk_boss_godhand" };

    public static readonly Dictionary<string, TrinketDef> AllTrinkets = new()
    {
        ["trk_sharp_tip"] = new() { Id = "trk_sharp_tip", Name = "Sharp Tip", Icon = "🔪", Tier = TrinketTier.Tier1, Desc = "+5 power for the run." },
        ["trk_thick_hide"] = new() { Id = "trk_thick_hide", Name = "Thick Hide", Icon = "🛡️", Tier = TrinketTier.Tier1, Desc = "+8% armor for the run." },
        ["trk_vitality"] = new() { Id = "trk_vitality", Name = "Vitality", Icon = "❤️", Tier = TrinketTier.Tier1, Desc = "+60 max HP for the run." },
        ["trk_lucky_penny"] = new() { Id = "trk_lucky_penny", Name = "Lucky Penny", Icon = "🪙", Tier = TrinketTier.Tier1, Desc = "+30% power-up charge gain." },
        ["trk_quick_reflex"] = new() { Id = "trk_quick_reflex", Name = "Quick Reflex", Icon = "💨", Tier = TrinketTier.Tier1, Desc = "Enemies have -10% accuracy." },
        ["trk_double_tap"] = new() { Id = "trk_double_tap", Name = "Double Tap", Icon = "✌️", Tier = TrinketTier.Tier2, Desc = "20% chance to deal double damage on a hit." },
        ["trk_vampiric"] = new() { Id = "trk_vampiric", Name = "Vampiric", Icon = "🩸", Tier = TrinketTier.Tier2, Desc = "Heal 3 HP for every dart that hits." },
        ["trk_bulwark"] = new() { Id = "trk_bulwark", Name = "Bulwark", Icon = "🏰", Tier = TrinketTier.Tier2, Desc = "Reduce every incoming hit by 5." },
        ["trk_eagle_eye"] = new() { Id = "trk_eagle_eye", Name = "Eagle Eye", Icon = "🦅", Tier = TrinketTier.Tier2, Desc = "15% chance to crit for +50% damage." },
        ["trk_phantom_step"] = new() { Id = "trk_phantom_step", Name = "Phantom Step", Icon = "👻", Tier = TrinketTier.Tier2, Desc = "12% chance to dodge an enemy dart." },
        ["trk_berserker"] = new() { Id = "trk_berserker", Name = "Berserker", Icon = "😤", Tier = TrinketTier.Tier3, Desc = "+15 power while below 30% HP." },
        ["trk_frozen_core"] = new() { Id = "trk_frozen_core", Name = "Frozen Core", Icon = "❄️", Tier = TrinketTier.Tier3, Desc = "25% chance to freeze an enemy for 1 turn on hit." },
        ["trk_overcharge"] = new() { Id = "trk_overcharge", Name = "Overcharge", Icon = "⚡", Tier = TrinketTier.Tier3, Desc = "Start every battle with 40% power-up charge." },
        ["trk_executioner"] = new() { Id = "trk_executioner", Name = "Executioner", Icon = "⚔️", Tier = TrinketTier.Tier3, Desc = "+50% damage to enemies below 25% HP." },
        ["trk_second_wind"] = new() { Id = "trk_second_wind", Name = "Second Wind", Icon = "🌬️", Tier = TrinketTier.Tier3, Desc = "Heal 15 HP when you defeat an enemy." },
        ["trk_giants_belt"] = new() { Id = "trk_giants_belt", Name = "Giant's Belt", Icon = "🏋️", Tier = TrinketTier.Tier4, Desc = "+50% max HP for the run." },
        ["trk_soul_harvest"] = new() { Id = "trk_soul_harvest", Name = "Soul Harvest", Icon = "💀", Tier = TrinketTier.Tier4, Desc = "+50% XP from kills." },
        ["trk_chain_lightning"] = new() { Id = "trk_chain_lightning", Name = "Chain Lightning", Icon = "🌩️", Tier = TrinketTier.Tier4, Desc = "Hits splash 25% damage to another enemy." },
        ["trk_adamant"] = new() { Id = "trk_adamant", Name = "Adamant", Icon = "💠", Tier = TrinketTier.Tier4, Desc = "Ignore the first enemy hit each round." },
        ["trk_phoenix_heart"] = new() { Id = "trk_phoenix_heart", Name = "Phoenix Heart", Icon = "🔥", Tier = TrinketTier.Tier4, Desc = "Revive once per run at 25% HP." },
        ["trk_boss_warlords_crown"] = new() { Id = "trk_boss_warlords_crown", Name = "Warlord's Crown", Icon = "👑", Tier = TrinketTier.Tier4, Desc = "+25 power for the run." },
        ["trk_boss_ice_crystal"] = new() { Id = "trk_boss_ice_crystal", Name = "Ice Crystal", Icon = "💎", Tier = TrinketTier.Tier4, Desc = "+15% armor for the run." },
        ["trk_boss_verdant_seed"] = new() { Id = "trk_boss_verdant_seed", Name = "Verdant Seed", Icon = "🌱", Tier = TrinketTier.Tier4, Desc = "+200 max HP for the run." },
        ["trk_boss_dragon_heart"] = new() { Id = "trk_boss_dragon_heart", Name = "Dragon Heart", Icon = "🐉", Tier = TrinketTier.Tier4, Desc = "+40 power for the run." },
        ["trk_boss_frost_throne"] = new() { Id = "trk_boss_frost_throne", Name = "Frost Throne", Icon = "🪑", Tier = TrinketTier.Tier4, Desc = "+25% armor for the run." },
        ["trk_boss_maw_jaw"] = new() { Id = "trk_boss_maw_jaw", Name = "Maw Jaw", Icon = "🦷", Tier = TrinketTier.Tier4, Desc = "+400 max HP for the run." },
        ["trk_boss_void_cloak"] = new() { Id = "trk_boss_void_cloak", Name = "Void Cloak", Icon = "🌀", Tier = TrinketTier.Tier4, Desc = "+60 power for the run." },
        ["trk_boss_eternal_flame"] = new() { Id = "trk_boss_eternal_flame", Name = "Eternal Flame", Icon = "🌋", Tier = TrinketTier.Tier4, Desc = "+35% armor for the run." },
        ["trk_boss_titan_heart"] = new() { Id = "trk_boss_titan_heart", Name = "Titan Heart", Icon = "🗿", Tier = TrinketTier.Tier4, Desc = "+600 max HP for the run." },
        ["trk_boss_godhand"] = new() { Id = "trk_boss_godhand", Name = "Godhand", Icon = "✋", Tier = TrinketTier.Tier4, Desc = "+100 power for the run." },
    };

    public static TrinketDef? GetTrinket(string id) => AllTrinkets.TryGetValue(id, out var t) ? t : null;
    public static List<string> AllTrinketIds => AllTrinkets.Keys.ToList();

    public static List<string> BossTrinketOptions(int bossNumber)
    {
        if (bossNumber == 1) return BossTrinketPool1.ToList();
        if (bossNumber == 2) return BossTrinketPool2.ToList();
        return BossTrinketPoolRest.ToList();
    }

    public static List<string> AvailablePool(int miniBossesDefeated, int bossesDefeated)
    {
        var pool = StarterPool.ToList();
        for (int i = 0; i < miniBossesDefeated && i < MinibossPool.Length; i++) pool.Add(MinibossPool[i]);
        for (int i = 0; i < bossesDefeated && i < BossPool.Length; i++) pool.Add(BossPool[i]);
        return pool;
    }

    public static string? NewlyUnlockedTrinket(int miniBossesDefeated, int bossesDefeated)
    {
        if (bossesDefeated > 0 && bossesDefeated <= BossPool.Length)
            return BossPool[bossesDefeated - 1];
        if (miniBossesDefeated > 0 && miniBossesDefeated <= MinibossPool.Length)
            return MinibossPool[miniBossesDefeated - 1];
        return null;
    }
}
