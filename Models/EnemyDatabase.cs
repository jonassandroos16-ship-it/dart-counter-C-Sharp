namespace dart_counter.Models;

public static class EnemyDatabase
{
    public static readonly Dictionary<string, EnemyDef> All = new()
    {
        // Chapter 1 — Crimson Vale (Easy)
        ["goblin_scout"] = new() { Name = "Goblin Scout", Difficulty = Difficulty.Easy, MaxHp = 30, Armor = 0, Accuracy = 0.20, Precision = 0.25, Shields = new() },
        ["goblin_brute"] = new() { Name = "Goblin Brute", Difficulty = Difficulty.Easy, MaxHp = 50, Armor = 0, Accuracy = 0.25, Precision = 0.20, Shields = new() },
        ["orc_raider"] = new() { Name = "Orc Raider", Difficulty = Difficulty.Easy, MaxHp = 80, Armor = 2, Accuracy = 0.30, Precision = 0.35, Shields = new() },
        ["dark_mage"] = new() { Name = "Dark Mage", Difficulty = Difficulty.Easy, MaxHp = 70, Armor = 0, Accuracy = 0.35, Precision = 0.40, Shields = new() },
        ["royal_guard"] = new() { Name = "Royal Guard", Difficulty = Difficulty.Easy, MaxHp = 90, Armor = 3, Accuracy = 0.30, Precision = 0.30, Shields = new() },
        ["warlord_malakar"] = new() { Name = "Warlord Malakar", Difficulty = Difficulty.Boss, MaxHp = 180, Armor = 4, Accuracy = 0.45, Precision = 0.50, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" } } },

        // Chapter 2 — Frozen Throne (Medium)
        ["ice_wolf"] = new() { Name = "Ice Wolf", Difficulty = Difficulty.Easy, MaxHp = 70, Armor = 2, Accuracy = 0.40, Precision = 0.40, Shields = new() },
        ["frost_archer"] = new() { Name = "Frost Archer", Difficulty = Difficulty.Hard, MaxHp = 110, Armor = 4, Accuracy = 0.55, Precision = 0.58, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "BOTTOM_HALF" } } },
        ["frost_knight"] = new() { Name = "Frost Knight", Difficulty = Difficulty.Hard, MaxHp = 160, Armor = 8, Accuracy = 0.50, Precision = 0.48, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" }, new() { Type = ShieldType.Exact, TargetValue = "T20" } } },
        ["ice_queen"] = new() { Name = "The Ice Queen", Difficulty = Difficulty.Boss, MaxHp = 320, Armor = 12, Accuracy = 0.70, Precision = 0.75, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" }, new() { Type = ShieldType.Exact, TargetValue = "D20" }, new() { Type = ShieldType.Exact, TargetValue = "Bull" } } },

        // Chapter 3 — Verdant Maw (Hard)
        ["vine_lasher"] = new() { Name = "Vine Lasher", Difficulty = Difficulty.Hard, MaxHp = 120, Armor = 4, Accuracy = 0.60, Precision = 0.60, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" } } },
        ["spore_bloom"] = new() { Name = "Spore Bloom", Difficulty = Difficulty.Hard, MaxHp = 180, Armor = 6, Accuracy = 0.68, Precision = 0.65, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" }, new() { Type = ShieldType.Exact, TargetValue = "D18" } } },
        ["thorn_spearman"] = new() { Name = "Thorn Spearman", Difficulty = Difficulty.Hard, MaxHp = 240, Armor = 12, Accuracy = 0.72, Precision = 0.70, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "BOTTOM_HALF" }, new() { Type = ShieldType.Exact, TargetValue = "T19" } } },
        ["bloom_warden"] = new() { Name = "Bloom Warden", Difficulty = Difficulty.Hard, MaxHp = 300, Armor = 16, Accuracy = 0.78, Precision = 0.75, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" }, new() { Type = ShieldType.Exact, TargetValue = "T20" }, new() { Type = ShieldType.Exact, TargetValue = "D19" } } },
        ["the_verdant_maw"] = new() { Name = "The Verdant Maw", Difficulty = Difficulty.Boss, MaxHp = 600, Armor = 22, Accuracy = 0.85, Precision = 0.88, Shields = new() { new() { Type = ShieldType.Span, TargetValue = "TOP_HALF" }, new() { Type = ShieldType.Exact, TargetValue = "D20" }, new() { Type = ShieldType.Exact, TargetValue = "Bull" }, new() { Type = ShieldType.Exact, TargetValue = "T20" } } },
    };
}
