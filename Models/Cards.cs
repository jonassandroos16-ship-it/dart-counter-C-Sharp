using System.Text.Json.Serialization;

namespace dart_counter.Models;

public enum CardType { Damage, Spell, Utility }
public enum CardMode { Competitive, Coop, Both }
public enum CardClass { Warrior, Priest, Rogue, Any }
public enum CardRarity { Common, Rare, Epic }

public class CardDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public CardType Type { get; set; }
    public CardMode Mode { get; set; }
    public CardClass Class { get; set; }
    public CardRarity Rarity { get; set; }
    public string Desc { get; set; } = "";
    public int? Base { get; set; }
    public int? Mult { get; set; }
    public string? Effect { get; set; }
    public int? Magnitude { get; set; }
    public bool Upgraded { get; set; }
    public int LevelRequired { get; set; } = 1;
}

public class PlayerCard
{
    public string CardId { get; set; } = "";
    public int UpgradeLevel { get; set; }
    public bool Upgraded { get; set; }
}

public class CardPlayState
{
    public List<PlayerCard> Deck { get; set; } = new();
    public List<PlayerCard> Hand { get; set; } = new();
    public List<PlayerCard> Used { get; set; } = new();
    public List<PlayerCard> Graveyard { get; set; } = new();
}

public static class CardDefinitions
{
    public static readonly List<CardDef> CardDefs = new()
    {
        new() { Id = "dmg_s20", Name = "Single 20", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Deal 20 damage.", Base = 20, Mult = 1, LevelRequired = 1 },
        new() { Id = "dmg_d20", Name = "Double 20", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Deal 40 damage.", Base = 20, Mult = 2, LevelRequired = 1 },
        new() { Id = "dmg_outer_bull", Name = "Outer Bull", Icon = "🟢", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Deal 25 damage.", Base = 25, Mult = 1, LevelRequired = 1 },
        new() { Id = "util_reroll", Name = "Reroll", Icon = "🎲", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Reroll your lowest dart throw this visit.", Effect = "reroll", LevelRequired = 1 },
        new() { Id = "util_reserve", Name = "Reserve", Icon = "📥", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Reserve 1 modifier card for a future turn.", Effect = "reserve", LevelRequired = 1 },
        new() { Id = "dmg_warrior_slam", Name = "Mighty Slam", Icon = "⚔️", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Common, Desc = "Deal 30 damage with brute force.", Base = 30, Mult = 1, LevelRequired = 1 },
        new() { Id = "dmg_warrior_cleave", Name = "Cleave", Icon = "🪓", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Common, Desc = "Deal 45 damage with a wide swing.", Base = 45, Mult = 1, LevelRequired = 1 },
        new() { Id = "spell_surge", Name = "Surge", Icon = "⚡", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Common, Desc = "Your next visit scores double.", Effect = "surge", Magnitude = 2, LevelRequired = 1 },
        new() { Id = "spell_hot_streak", Name = "Hot Streak", Icon = "🔥", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Common, Desc = "+5 per dart cumulative bonus next visit.", Effect = "hot_streak", Magnitude = 5, LevelRequired = 1 },
        new() { Id = "util_warrior_rage", Name = "Battle Rage", Icon = "💢", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Common, Desc = "Draw 2 extra cards next turn.", Effect = "draw", Magnitude = 2, LevelRequired = 1 },
        new() { Id = "dmg_priest_smite", Name = "Holy Smite", Icon = "✨", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Common, Desc = "Deal 35 damage with divine power.", Base = 35, Mult = 1, LevelRequired = 1 },
        new() { Id = "dmg_priest_judgment", Name = "Divine Judgment", Icon = "⚖️", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Common, Desc = "Deal 50 damage with holy judgment.", Base = 50, Mult = 1, LevelRequired = 1 },
        new() { Id = "spell_heal", Name = "Healing Light", Icon = "✨", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Common, Desc = "Restore 80 HP to the party.", Effect = "heal", Magnitude = 80, LevelRequired = 1 },
        new() { Id = "spell_accuracy_buff", Name = "Eagle Eye", Icon = "🦅", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Common, Desc = "Party gains +20% accuracy for 3 turns.", Effect = "accuracy_buff", Magnitude = 20, LevelRequired = 1 },
        new() { Id = "util_priest_blessing", Name = "Divine Blessing", Icon = "🙏", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Common, Desc = "Restore 40 HP and draw 1 extra card.", Effect = "blessing", Magnitude = 40, LevelRequired = 1 },
        new() { Id = "dmg_rogue_backstab", Name = "Backstab", Icon = "🗡️", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Common, Desc = "Deal 40 damage from the shadows.", Base = 40, Mult = 1, LevelRequired = 1 },
        new() { Id = "dmg_rogue_poison", Name = "Poison Strike", Icon = "🐍", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Common, Desc = "Deal 30 damage plus 10 poison next turn.", Base = 30, Mult = 1, LevelRequired = 1 },
        new() { Id = "spell_enemy_debuff", Name = "Weaken", Icon = "💀", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Common, Desc = "Enemies deal -30% damage for 2 turns.", Effect = "enemy_debuff", Magnitude = 30, LevelRequired = 1 },
        new() { Id = "spell_freeze", Name = "Frost Nova", Icon = "❄️", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Common, Desc = "Freeze all enemies for 1 turn.", Effect = "freeze", LevelRequired = 1 },
        new() { Id = "util_rogue_shadowstep", Name = "Shadowstep", Icon = "🌑", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Common, Desc = "Draw 1 extra card and swap a used card back.", Effect = "shadowstep", Magnitude = 1, LevelRequired = 1 },
        new() { Id = "dmg_t20", Name = "Triple 20", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Deal 60 damage.", Base = 20, Mult = 3, LevelRequired = 2 },
        new() { Id = "dmg_t19", Name = "Triple 19", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Deal 57 damage.", Base = 19, Mult = 3, LevelRequired = 2 },
        new() { Id = "dmg_t18", Name = "Triple 18", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Deal 54 damage.", Base = 18, Mult = 3, LevelRequired = 2 },
        new() { Id = "dmg_bull", Name = "Bullseye", Icon = "🐂", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Deal 50 damage.", Base = 50, Mult = 1, LevelRequired = 2 },
        new() { Id = "dmg_s19", Name = "Single 19", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Deal 19 damage.", Base = 19, Mult = 1, LevelRequired = 2 },
        new() { Id = "dmg_s18", Name = "Single 18", Icon = "🎯", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Common, Desc = "Deal 18 damage.", Base = 18, Mult = 1, LevelRequired = 2 },
        new() { Id = "spell_bust_protect", Name = "Bust Protect", Icon = "🛡️", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Prevents going over 0 if you overscore.", Effect = "bust_protect", LevelRequired = 2 },
        new() { Id = "spell_double_up", Name = "Double Up", Icon = "🔁", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Forces an opponent's next Double to count as a miss.", Effect = "double_up", LevelRequired = 2 },
        new() { Id = "util_draw", Name = "Quick Draw", Icon = "🃏", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Rare, Desc = "Draw an extra card next turn.", Effect = "draw", Magnitude = 1, LevelRequired = 2 },
        new() { Id = "dmg_warrior_strike", Name = "Warrior Strike", Icon = "⚔️", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Rare, Desc = "Deal 65 damage with warrior power.", Base = 20, Mult = 3, LevelRequired = 3 },
        new() { Id = "dmg_priest_smite_greater", Name = "Greater Smite", Icon = "✨", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Rare, Desc = "Deal 55 damage with divine power.", Base = 50, Mult = 1, LevelRequired = 3 },
        new() { Id = "dmg_rogue_assassinate", Name = "Assassinate", Icon = "🥷", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Rare, Desc = "Deal 70 damage with lethal precision.", Base = 70, Mult = 1, LevelRequired = 3 },
        new() { Id = "spell_power_buff", Name = "Power Infusion", Icon = "💪", Type = CardType.Spell, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Rare, Desc = "Party gains +10 power for 3 turns.", Effect = "power_buff", Magnitude = 10, LevelRequired = 3 },
        new() { Id = "util_shield", Name = "Party Shield", Icon = "🛡️", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Rare, Desc = "Party takes 50% less damage for 2 turns.", Effect = "party_shield", Magnitude = 50, LevelRequired = 3 },
        new() { Id = "dmg_meteor", Name = "Meteor Strike", Icon = "☄️", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Epic, Desc = "Deal 80 damage in a blazing impact.", Base = 80, Mult = 1, LevelRequired = 4 },
        new() { Id = "dmg_warrior_cleave_epic", Name = "Whirlwind", Icon = "🌀", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Epic, Desc = "Deal 90 damage with a mighty whirlwind.", Base = 90, Mult = 1, LevelRequired = 4 },
        new() { Id = "dmg_priest_judgment_epic", Name = "Armageddon", Icon = "🌋", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Epic, Desc = "Deal 85 damage with holy judgment.", Base = 85, Mult = 1, LevelRequired = 4 },
        new() { Id = "dmg_rogue_assassinate_epic", Name = "Death Strike", Icon = "💀", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Rogue, Rarity = CardRarity.Epic, Desc = "Deal 95 damage with lethal precision.", Base = 95, Mult = 1, LevelRequired = 4 },
        new() { Id = "util_extra_dart", Name = "Extra Throw", Icon = "➕", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Warrior, Rarity = CardRarity.Epic, Desc = "Gain an extra dart throw this turn.", Effect = "extra_dart", LevelRequired = 4 },
        new() { Id = "dmg_apocalypse", Name = "Apocalypse", Icon = "🌋", Type = CardType.Damage, Mode = CardMode.Both, Class = CardClass.Any, Rarity = CardRarity.Epic, Desc = "Deal 120 damage. The board trembles.", Base = 120, Mult = 1, LevelRequired = 5 },
        new() { Id = "util_revive", Name = "Phoenix Heart", Icon = "❤️", Type = CardType.Utility, Mode = CardMode.Both, Class = CardClass.Priest, Rarity = CardRarity.Epic, Desc = "Revive the party to 25% HP once.", Effect = "revive", LevelRequired = 5 },
    };

    private static readonly Dictionary<string, CardDef> _map = CardDefs.ToDictionary(c => c.Id);

    public static CardDef? GetCard(string id) => _map.TryGetValue(id, out var c) ? c : null;

    public static List<CardDef> CardsForMode(CardMode mode) =>
        CardDefs.Where(c => c.Mode == mode || c.Mode == CardMode.Both).ToList();

    public static List<CardDef> CardsForClass(CardClass cls, CardMode mode) =>
        CardDefs.Where(c => (c.Mode == mode || c.Mode == CardMode.Both) && (c.Class == cls || c.Class == CardClass.Any)).ToList();

    public static CardDef UpgradedCardDef(CardDef card)
    {
        var name = card.Name.TrimEnd('+');
        if (card.Type == CardType.Damage)
        {
            var baseVal = card.Base ?? 0;
            var mult = card.Mult ?? 1;
            var originalDmg = baseVal * mult;
            var upgradedDmg = (int)Math.Round(originalDmg * 1.5);
            return new CardDef
            {
                Id = card.Id, Name = name + "+", Icon = card.Icon, Type = card.Type,
                Mode = card.Mode, Class = card.Class, Rarity = card.Rarity,
                Desc = card.Desc, Upgraded = true,
                Base = mult > 1 ? (int)Math.Round(baseVal * 1.5) : upgradedDmg,
                Mult = mult > 1 ? mult : 1,
                LevelRequired = card.LevelRequired,
            };
        }
        return new CardDef
        {
            Id = card.Id, Name = name + "+", Icon = card.Icon, Type = card.Type,
            Mode = card.Mode, Class = card.Class, Rarity = card.Rarity,
            Desc = card.Desc, Upgraded = true,
            Effect = card.Effect,
            Magnitude = card.Magnitude.HasValue ? (int)Math.Round(card.Magnitude.Value * 1.5) : card.Magnitude,
            LevelRequired = card.LevelRequired,
        };
    }

    public static int CardDamage(CardDef card)
    {
        if (card.Type != CardType.Damage) return 0;
        return (card.Base ?? 0) * (card.Mult ?? 1);
    }

    public static string CardRarityColor(CardRarity rarity) => rarity switch
    {
        CardRarity.Common => "var(--border)",
        CardRarity.Rare => "#3b82f6",
        CardRarity.Epic => "#f59e0b",
        _ => "var(--border)",
    };

    public static string CardTypeColor(CardType type) => type switch
    {
        CardType.Damage => "#ef4444",
        CardType.Spell => "#3b82f6",
        CardType.Utility => "#10b981",
        _ => "#ef4444",
    };

    public static Dictionary<int, List<CardDef>> CardsByLevel(List<CardDef> cards)
    {
        return cards.GroupBy(c => c.LevelRequired).ToDictionary(g => g.Key, g => g.ToList());
    }
}
