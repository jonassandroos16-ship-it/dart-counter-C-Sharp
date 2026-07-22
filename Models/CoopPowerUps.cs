namespace dart_counter.Models;

public static class CoopPowerUps
{
    public static readonly List<CoopPowerUpDef> All = new()
    {
        // Starter
        new() { Id = "coop_heal", Name = "Heal", Icon = "❤️", Desc = "Restore 80 party HP instantly.", Cost = 100, Tier = CoopPowerUpTier.Starter },
        new() { Id = "coop_buff_power", Name = "Power Buff", Icon = "⚡", Desc = "All players +10 power for 3 turns.", Cost = 80, Tier = CoopPowerUpTier.Starter },
        new() { Id = "coop_buff_acc", Name = "Focus Buff", Icon = "🎯", Desc = "Distract all enemies — -20% accuracy & precision for 3 turns.", Cost = 80, Tier = CoopPowerUpTier.Starter },
        new() { Id = "coop_freeze", Name = "Freeze", Icon = "❄️", Desc = "Freeze all enemies for 2 turns — they cannot attack.", Cost = 100, Tier = CoopPowerUpTier.Starter },
        new() { Id = "coop_shield", Name = "Party Shield", Icon = "🛡️", Desc = "Absorb the next 40 party damage from enemies.", Cost = 70, Tier = CoopPowerUpTier.Starter },
        // Advanced — Chapter 1
        new() { Id = "coop_meteor", Name = "Meteor Strike", Icon = "☄️", Desc = "Rain fire on every enemy — 60 damage to each, ignoring shields.", Cost = 90, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_phantom", Name = "Phantom Darts", Icon = "👻", Desc = "Your next 3 darts auto-hit bullseye (50 each) on the targeted enemy.", Cost = 80, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_time_warp", Name = "Time Warp", Icon = "⏳", Desc = "Enemies take 50% more damage from all sources for 3 turns.", Cost = 110, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_ressurect", Name = "Resurrection", Icon = "✨", Desc = "Restore the party to full HP and clear all enemy shields.", Cost = 130, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_apocalypse", Name = "Apocalypse", Icon = "🔥", Desc = "BOSS REWARD: 150 damage to every enemy, freeze them for 2 turns, and fully heal the party.", Cost = 150, Tier = CoopPowerUpTier.Advanced },
        // Advanced — Chapter 2
        new() { Id = "coop_blizzard", Name = "Blizzard", Icon = "🌨️", Desc = "A howling gale — 45 damage to every enemy and freeze them for 1 turn.", Cost = 95, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_frostbite", Name = "Frostbite", Icon = "🥶", Desc = "Chill every enemy to the bone — 40 damage and -25% accuracy for 3 turns.", Cost = 100, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_ice_lance", Name = "Ice Lance", Icon = "🔱", Desc = "A single perfect shard — 120 damage to the targeted enemy, ignoring shields.", Cost = 90, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_winter_veil", Name = "Winter's Veil", Icon = "🌫️", Desc = "Wrap the party in mist — restore 60 HP and shield against the next 2 turns of damage.", Cost = 120, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_glacial_doom", Name = "Glacial Doom", Icon = "🧊", Desc = "BOSS REWARD: 180 damage to every enemy, freeze them for 3 turns, and fully heal the party.", Cost = 160, Tier = CoopPowerUpTier.Advanced },
        // Advanced — Chapter 3
        new() { Id = "coop_vine_grasp", Name = "Vine Grasp", Icon = "🌿", Desc = "Root every enemy — 50 damage and freeze them for 1 turn.", Cost = 100, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_spore_burst", Name = "Spore Burst", Icon = "🍄", Desc = "A choking cloud — 60 damage and -30% accuracy to every enemy for 3 turns.", Cost = 110, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_thorn_lance", Name = "Thorn Lance", Icon = "🌵", Desc = "A single perfect thorn — 160 damage to the targeted enemy, ignoring shields.", Cost = 100, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_verdant_bloom", Name = "Verdant Bloom", Icon = "🌸", Desc = "Restore 100 party HP, clear all enemy shields, and grant the party +5 power for 3 turns.", Cost = 140, Tier = CoopPowerUpTier.Advanced },
        new() { Id = "coop_heart_of_maw", Name = "Heart of the Maw", Icon = "🫀", Desc = "BOSS REWARD: 220 damage to every enemy, freeze them for 3 turns, clear their shields, and fully heal the party.", Cost = 180, Tier = CoopPowerUpTier.Advanced },
    };

    public static CoopPowerUpDef? Get(string? id) => string.IsNullOrEmpty(id) ? null : All.FirstOrDefault(p => p.Id == id);

    public static List<string> UnlockedFor(PlayerCampaignProgress? progress)
    {
        var starter = All.Where(p => p.Tier == CoopPowerUpTier.Starter).Select(p => p.Id).ToList();
        var advanced = progress?.UnlockedPowerUps ?? new();
        return starter.Concat(advanced).ToList();
    }
}
