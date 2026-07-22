namespace dart_counter.Models;

public static class CoopClasses
{
    public static readonly List<CoopClassDef> All = new()
    {
        new() { Id = CoopClassId.Warrior, Name = "Warrior", Icon = "⚔️", Desc = "A frontline striker. Grants the party flat POWER bonuses, making every dart hit harder.", StarterPassive = "war_power_1" },
        new() { Id = CoopClassId.Priest, Name = "Priest", Icon = "✨", Desc = "A guardian healer. Grants the party flat MAX HP bonuses, so the party can soak more damage.", StarterPassive = "pri_hp_1" },
        new() { Id = CoopClassId.Rogue, Name = "Rogue", Icon = "🗡️", Desc = "A nimble defender. Grants the party flat ARMOR bonuses, reducing every enemy dart.", StarterPassive = "rog_armor_1" },
    };

    public static readonly List<CoopPassiveDef> Passives = new()
    {
        // Warrior — tier 1
        new() { Id = "war_power_1", ClassId = CoopClassId.Warrior, Tier = 1, Name = "Battle Cry", Icon = "⚔️", Desc = "Party +3 power (flat per dart).", Power = 3, LevelRequired = 1 },
        new() { Id = "war_crit_1", ClassId = CoopClassId.Warrior, Tier = 1, Name = "Keen Edge", Icon = "🎯", Desc = "Party +2 power and +1 armor (flat).", Power = 2, Armor = 1, LevelRequired = 1 },
        new() { Id = "war_fury_1", ClassId = CoopClassId.Warrior, Tier = 1, Name = "Iron Will", Icon = "💪", Desc = "Party +2 power and +30 max HP.", Power = 2, Health = 30, LevelRequired = 1 },
        // Warrior — tier 2
        new() { Id = "war_power_2", ClassId = CoopClassId.Warrior, Tier = 2, Name = "War Banner", Icon = "🚩", Desc = "Party +8 power (flat per dart).", Power = 8, LevelRequired = 2 },
        new() { Id = "war_crit_2", ClassId = CoopClassId.Warrior, Tier = 2, Name = "Bloodlust", Icon = "🩸", Desc = "Party +6 power and +2 armor (flat).", Power = 6, Armor = 2, LevelRequired = 2 },
        new() { Id = "war_fury_2", ClassId = CoopClassId.Warrior, Tier = 2, Name = "Raging Roar", Icon = "🦁", Desc = "Party +6 power and +80 max HP.", Power = 6, Health = 80, LevelRequired = 2 },
        // Warrior — tier 3
        new() { Id = "war_power_3", ClassId = CoopClassId.Warrior, Tier = 3, Name = "Berserker Aura", Icon = "🔥", Desc = "Party +15 power (flat per dart).", Power = 15, LevelRequired = 3 },
        new() { Id = "war_crit_3", ClassId = CoopClassId.Warrior, Tier = 3, Name = "Executioner", Icon = "🪓", Desc = "Party +12 power and +4 armor (flat).", Power = 12, Armor = 4, LevelRequired = 3 },
        new() { Id = "war_fury_3", ClassId = CoopClassId.Warrior, Tier = 3, Name = "Unbreakable", Icon = "🛡️", Desc = "Party +12 power and +180 max HP.", Power = 12, Health = 180, LevelRequired = 3 },
        // Warrior — tier 4
        new() { Id = "war_power_4", ClassId = CoopClassId.Warrior, Tier = 4, Name = "Warlord's Roar", Icon = "🐉", Desc = "Party +22 power (flat per dart).", Power = 22, LevelRequired = 4 },
        new() { Id = "war_crit_4", ClassId = CoopClassId.Warrior, Tier = 4, Name = "Crimson Reaper", Icon = "🩸", Desc = "Party +18 power and +6 armor (flat).", Power = 18, Armor = 6, LevelRequired = 4 },
        new() { Id = "war_fury_4", ClassId = CoopClassId.Warrior, Tier = 4, Name = "Iron Tide", Icon = "🌊", Desc = "Party +18 power and +260 max HP.", Power = 18, Health = 260, LevelRequired = 4 },
        // Warrior — tier 5
        new() { Id = "war_power_5", ClassId = CoopClassId.Warrior, Tier = 5, Name = "Apex Predator", Icon = "🦖", Desc = "Party +32 power (flat per dart).", Power = 32, LevelRequired = 5 },
        new() { Id = "war_crit_5", ClassId = CoopClassId.Warrior, Tier = 5, Name = "Doombringer", Icon = "☠️", Desc = "Party +26 power and +9 armor (flat).", Power = 26, Armor = 9, LevelRequired = 5 },
        new() { Id = "war_fury_5", ClassId = CoopClassId.Warrior, Tier = 5, Name = "Titan's Vigor", Icon = "🗿", Desc = "Party +26 power and +380 max HP.", Power = 26, Health = 380, LevelRequired = 5 },

        // Priest — tier 1
        new() { Id = "pri_hp_1", ClassId = CoopClassId.Priest, Tier = 1, Name = "Blessing", Icon = "✨", Desc = "Party +60 max HP.", Health = 60, LevelRequired = 1 },
        new() { Id = "pri_regen_1", ClassId = CoopClassId.Priest, Tier = 1, Name = "Mending", Icon = "💧", Desc = "Party +40 max HP and +1 armor (flat).", Health = 40, Armor = 1, LevelRequired = 1 },
        new() { Id = "pri_shield_1", ClassId = CoopClassId.Priest, Tier = 1, Name = "Ward", Icon = "🧿", Desc = "Party +40 max HP and +2 power (flat).", Health = 40, Power = 2, LevelRequired = 1 },
        // Priest — tier 2
        new() { Id = "pri_hp_2", ClassId = CoopClassId.Priest, Tier = 2, Name = "Sanctuary", Icon = "🙏", Desc = "Party +150 max HP.", Health = 150, LevelRequired = 2 },
        new() { Id = "pri_regen_2", ClassId = CoopClassId.Priest, Tier = 2, Name = "Holy Renewal", Icon = "🌿", Desc = "Party +110 max HP and +3 armor (flat).", Health = 110, Armor = 3, LevelRequired = 2 },
        new() { Id = "pri_shield_2", ClassId = CoopClassId.Priest, Tier = 2, Name = "Sacred Barrier", Icon = "🔰", Desc = "Party +110 max HP and +5 power (flat).", Health = 110, Power = 5, LevelRequired = 2 },
        // Priest — tier 3
        new() { Id = "pri_hp_3", ClassId = CoopClassId.Priest, Tier = 3, Name = "Divine Aegis", Icon = "😇", Desc = "Party +300 max HP.", Health = 300, LevelRequired = 3 },
        new() { Id = "pri_regen_3", ClassId = CoopClassId.Priest, Tier = 3, Name = "Eternal Spring", Icon = "⛲", Desc = "Party +240 max HP and +6 armor (flat).", Health = 240, Armor = 6, LevelRequired = 3 },
        new() { Id = "pri_shield_3", ClassId = CoopClassId.Priest, Tier = 3, Name = "Celestial Bulwark", Icon = "🌟", Desc = "Party +240 max HP and +10 power (flat).", Health = 240, Power = 10, LevelRequired = 3 },
        // Priest — tier 4
        new() { Id = "pri_hp_4", ClassId = CoopClassId.Priest, Tier = 4, Name = "Heaven's Embrace", Icon = "🙏", Desc = "Party +440 max HP.", Health = 440, LevelRequired = 4 },
        new() { Id = "pri_regen_4", ClassId = CoopClassId.Priest, Tier = 4, Name = "Lifewell Spring", Icon = "💧", Desc = "Party +340 max HP and +9 armor (flat).", Health = 340, Armor = 9, LevelRequired = 4 },
        new() { Id = "pri_shield_4", ClassId = CoopClassId.Priest, Tier = 4, Name = "Seraphic Aegis", Icon = "🛡️", Desc = "Party +340 max HP and +15 power (flat).", Health = 340, Power = 15, LevelRequired = 4 },
        // Priest — tier 5
        new() { Id = "pri_hp_5", ClassId = CoopClassId.Priest, Tier = 5, Name = "Eternal Covenant", Icon = "✨", Desc = "Party +640 max HP.", Health = 640, LevelRequired = 5 },
        new() { Id = "pri_regen_5", ClassId = CoopClassId.Priest, Tier = 5, Name = "Font of Life", Icon = "⛲", Desc = "Party +500 max HP and +13 armor (flat).", Health = 500, Armor = 13, LevelRequired = 5 },
        new() { Id = "pri_shield_5", ClassId = CoopClassId.Priest, Tier = 5, Name = "Radiant Bastion", Icon = "🌟", Desc = "Party +500 max HP and +22 power (flat).", Health = 500, Power = 22, LevelRequired = 5 },

        // Rogue — tier 1
        new() { Id = "rog_armor_1", ClassId = CoopClassId.Rogue, Tier = 1, Name = "Light Steps", Icon = "🗡️", Desc = "Party +2 armor (flat per enemy dart).", Armor = 2, LevelRequired = 1 },
        new() { Id = "rog_dodge_1", ClassId = CoopClassId.Rogue, Tier = 1, Name = "Nimble", Icon = "💨", Desc = "Party +1 armor and +30 max HP.", Armor = 1, Health = 30, LevelRequired = 1 },
        new() { Id = "rog_thorns_1", ClassId = CoopClassId.Rogue, Tier = 1, Name = "Bristling", Icon = "🌵", Desc = "Party +1 armor and +2 power (flat).", Armor = 1, Power = 2, LevelRequired = 1 },
        // Rogue — tier 2
        new() { Id = "rog_armor_2", ClassId = CoopClassId.Rogue, Tier = 2, Name = "Shadow Veil", Icon = "🌫️", Desc = "Party +5 armor (flat per enemy dart).", Armor = 5, LevelRequired = 2 },
        new() { Id = "rog_dodge_2", ClassId = CoopClassId.Rogue, Tier = 2, Name = "Flicker", Icon = "⚡", Desc = "Party +3 armor and +80 max HP.", Armor = 3, Health = 80, LevelRequired = 2 },
        new() { Id = "rog_thorns_2", ClassId = CoopClassId.Rogue, Tier = 2, Name = "Razor Edge", Icon = "🔪", Desc = "Party +3 armor and +6 power (flat).", Armor = 3, Power = 6, LevelRequired = 2 },
        // Rogue — tier 3
        new() { Id = "rog_armor_3", ClassId = CoopClassId.Rogue, Tier = 3, Name = "Phantom Guard", Icon = "👻", Desc = "Party +10 armor (flat per enemy dart).", Armor = 10, LevelRequired = 3 },
        new() { Id = "rog_dodge_3", ClassId = CoopClassId.Rogue, Tier = 3, Name = "Afterimage", Icon = "🌀", Desc = "Party +7 armor and +180 max HP.", Armor = 7, Health = 180, LevelRequired = 3 },
        new() { Id = "rog_thorns_3", ClassId = CoopClassId.Rogue, Tier = 3, Name = "Spike Mail", Icon = "🦔", Desc = "Party +7 armor and +12 power (flat).", Armor = 7, Power = 12, LevelRequired = 3 },
        // Rogue — tier 4
        new() { Id = "rog_armor_4", ClassId = CoopClassId.Rogue, Tier = 4, Name = "Umbral Bulwark", Icon = "🌑", Desc = "Party +15 armor (flat per enemy dart).", Armor = 15, LevelRequired = 4 },
        new() { Id = "rog_dodge_4", ClassId = CoopClassId.Rogue, Tier = 4, Name = "Mirror Step", Icon = "🪞", Desc = "Party +11 armor and +260 max HP.", Armor = 11, Health = 260, LevelRequired = 4 },
        new() { Id = "rog_thorns_4", ClassId = CoopClassId.Rogue, Tier = 4, Name = "Razor Barb", Icon = "🔪", Desc = "Party +11 armor and +18 power (flat).", Armor = 11, Power = 18, LevelRequired = 4 },
        // Rogue — tier 5
        new() { Id = "rog_armor_5", ClassId = CoopClassId.Rogue, Tier = 5, Name = "Eclipse Aegis", Icon = "🌑", Desc = "Party +22 armor (flat per enemy dart).", Armor = 22, LevelRequired = 5 },
        new() { Id = "rog_dodge_5", ClassId = CoopClassId.Rogue, Tier = 5, Name = "Phantasm", Icon = "🌫️", Desc = "Party +16 armor and +380 max HP.", Armor = 16, Health = 380, LevelRequired = 5 },
        new() { Id = "rog_thorns_5", ClassId = CoopClassId.Rogue, Tier = 5, Name = "Maw of Thorns", Icon = "🐉", Desc = "Party +16 armor and +26 power (flat).", Armor = 16, Power = 26, LevelRequired = 5 },
    };

    public static CoopClassDef? GetClass(CoopClassId? id) => id == null ? null : All.FirstOrDefault(c => c.Id == id);
    public static CoopPassiveDef? GetPassive(string? id) => string.IsNullOrEmpty(id) ? null : Passives.FirstOrDefault(p => p.Id == id);
    public static List<CoopPassiveDef> PassivesForClass(CoopClassId classId) => Passives.Where(p => p.ClassId == classId).OrderBy(p => p.Tier).ToList();

    public static PartyPassiveBonus ComputePartyPassiveBonus(List<Player> players)
    {
        var bonus = new PartyPassiveBonus();
        foreach (var p in players)
        {
            var prog = p.CoopProgress;
            if (prog == null || prog.ClassId == null) continue;
            var equipped = prog.EquippedPassives ?? new();
            if (equipped.Count == 0) continue;
            foreach (var pid in equipped)
            {
                var def = GetPassive(pid);
                if (def == null) continue;
                bonus.Power += def.Power;
                bonus.Health += def.Health;
                bonus.Armor += def.Armor;
                bonus.Sources.Add(new()
                {
                    PlayerId = p.Id, PlayerName = p.Name,
                    PassiveName = def.Name, Icon = def.Icon,
                    Power = def.Power, Health = def.Health, Armor = def.Armor
                });
            }
        }
        return bonus;
    }

    public static List<string> UnlockedPassivesForPlayer(PlayerCoopProgress? prog, int playerLevel = 1)
    {
        if (prog == null || prog.ClassId == null) return new();
        return PassivesForClass(prog.ClassId.Value)
            .Where(p => playerLevel >= p.LevelRequired)
            .Select(p => p.Id).ToList();
    }

    public static PlayerCoopProgress DefaultCoopProgress() => new()
    {
        ClassId = CoopClassId.Warrior,
        UnlockedPassives = new(),
        EquippedPassives = new()
    };

    public static PlayerCoopProgress SelectClass(PlayerCoopProgress prog, CoopClassId classId)
    {
        var cls = GetClass(classId);
        var tier1Ids = Passives.Where(p => p.ClassId == classId && p.Tier == 1).Select(p => p.Id).ToList();
        var unlocked = new HashSet<string>(prog.UnlockedPassives ?? new());
        foreach (var id in tier1Ids) unlocked.Add(id);
        return new()
        {
            ClassId = classId,
            UnlockedPassives = unlocked.ToList(),
            EquippedPassives = string.IsNullOrEmpty(cls?.StarterPassive) ? new() : new() { cls.StarterPassive }
        };
    }

    public static PlayerCoopProgress EquipPassive(PlayerCoopProgress prog, string passiveId)
    {
        var def = GetPassive(passiveId);
        if (def == null || prog.ClassId == null || def.ClassId != prog.ClassId) return prog;
        var unlocked = UnlockedPassivesForPlayer(prog);
        if (!unlocked.Contains(passiveId)) return prog;
        var other = (prog.EquippedPassives ?? new()).Where(id => GetPassive(id)?.ClassId != def.ClassId).ToList();
        return new()
        {
            ClassId = prog.ClassId,
            UnlockedPassives = prog.UnlockedPassives,
            EquippedPassives = other.Append(passiveId).ToList()
        };
    }

    public static (PlayerCoopProgress progress, List<string> newlyUnlocked) ReconcilePassives(PlayerCoopProgress? prog, int playerLevel)
    {
        var cur = prog ?? DefaultCoopProgress();
        if (cur.ClassId == null) return (cur, new());
        var classPassives = PassivesForClass(cur.ClassId.Value);
        var newlyUnlocked = new List<string>();
        var unlockedSet = new HashSet<string>(cur.UnlockedPassives ?? new());
        foreach (var p in classPassives)
        {
            if (playerLevel >= p.LevelRequired && !unlockedSet.Contains(p.Id))
            {
                newlyUnlocked.Add(p.Id);
                unlockedSet.Add(p.Id);
            }
        }
        return (new() { ClassId = cur.ClassId, UnlockedPassives = unlockedSet.ToList(), EquippedPassives = cur.EquippedPassives }, newlyUnlocked);
    }

    public static int CoopXpForBattle(BattleStats stats, bool won)
    {
        var b = won ? 20 : 5;
        var dartBonus = Math.Min(20, stats.DartsThrown / 3);
        var defeatBonus = Math.Min(15, stats.EnemiesDefeated * 3);
        return b + dartBonus + defeatBonus;
    }
}
