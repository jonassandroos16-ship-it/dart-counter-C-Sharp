using System.Text.Json;

namespace dart_counter.Logic;

public static class CardDeck
{
    private static readonly string[] StarterSharedAttack = { "dmg_s20", "dmg_d20", "dmg_outer_bull" };
    private static readonly string[] StarterSharedUtility = { "util_reroll", "util_reserve" };

    private static readonly Dictionary<string, (string[] specific, string utility)> StarterClassCards = new()
    {
        ["warrior"] = (new[] { "dmg_warrior_slam", "dmg_warrior_cleave", "spell_surge", "spell_hot_streak" }, "util_warrior_rage"),
        ["priest"] = (new[] { "dmg_priest_smite", "dmg_priest_judgment", "spell_heal", "spell_accuracy_buff" }, "util_priest_blessing"),
        ["rogue"] = (new[] { "dmg_rogue_backstab", "dmg_rogue_poison", "spell_enemy_debuff", "spell_freeze" }, "util_rogue_shadowstep"),
    };

    private static readonly string[] StarterNoClassFallback =
    {
        "dmg_s20", "dmg_d20", "dmg_outer_bull", "dmg_s20",
        "util_reroll", "util_reserve", "util_reroll",
        "dmg_s20", "dmg_d20", "util_reserve",
    };

    public const int MaxUpgradeLevel = 5;
    public const int HandSize = 5;
    public const int MaxPlaysPerTurn = 3;

    public static List<PlayerCard> DefaultPlayerCards(string? classId)
    {
        var key = classId ?? "";
        if (!StarterClassCards.TryGetValue(key, out var cls))
            return StarterNoClassFallback.Select(id => new PlayerCard { CardId = id, UpgradeLevel = 0, Upgraded = false }).ToList();

        var cards = new List<PlayerCard>();
        foreach (var id in cls.specific) cards.Add(new() { CardId = id });
        foreach (var id in StarterSharedAttack) cards.Add(new() { CardId = id });
        foreach (var id in StarterSharedUtility) cards.Add(new() { CardId = id });
        cards.Add(new() { CardId = cls.utility });
        return cards;
    }

    public static string ClassKey(string? classId) => classId ?? "any";

    public static List<PlayerCard> GetPlayerCards(Player player)
    {
        if (player.Cards == null) return DefaultPlayerCards(player.CoopProgress?.ClassId?.ToString().ToLowerInvariant());
        var key = ClassKey(player.CoopProgress?.ClassId?.ToString().ToLowerInvariant());
        if (player.Cards.TryGetValue(key, out var deck) && deck.Count > 0) return deck;
        return DefaultPlayerCards(player.CoopProgress?.ClassId?.ToString().ToLowerInvariant());
    }

    public static bool HasCard(List<PlayerCard> cards, string cardId) => cards.Any(c => c.CardId == cardId);

    public static List<PlayerCard> AddCard(List<PlayerCard> cards, string cardId)
    {
        if (HasCard(cards, cardId)) return cards;
        var result = cards.ToList();
        result.Add(new() { CardId = cardId });
        return result;
    }

    public static List<PlayerCard> RemoveCard(List<PlayerCard> cards, string cardId) =>
        cards.Where(c => c.CardId != cardId).ToList();

    public static List<PlayerCard> UpgradeCard(List<PlayerCard> cards, string cardId) =>
        cards.Select(c => c.CardId == cardId ? new PlayerCard { CardId = c.CardId, UpgradeLevel = c.UpgradeLevel + 1, Upgraded = true } : c).ToList();

    public static bool CanUpgradeCard(List<PlayerCard> cards, string cardId) =>
        cards.FirstOrDefault(c => c.CardId == cardId) is { } pc && pc.UpgradeLevel < MaxUpgradeLevel;

    public static CardDef? ResolveCardDef(PlayerCard pc)
    {
        var def = CardDefinitions.GetCard(pc.CardId);
        if (def == null) return null;
        var result = def;
        for (int i = 0; i < pc.UpgradeLevel; i++)
            result = CardDefinitions.UpgradedCardDef(result);
        return result;
    }

    public static List<CardDef> CardsForLevelUp(string? classId, int level, CardMode mode, List<PlayerCard> ownedCards)
    {
        var cls = classId ?? "any";
        var pool = CardDefinitions.CardsForClass(
            cls == "warrior" ? CardClass.Warrior : cls == "priest" ? CardClass.Priest : cls == "rogue" ? CardClass.Rogue : CardClass.Any,
            mode);
        return pool.Where(c => c.LevelRequired <= level && !HasCard(ownedCards, c.Id)).ToList();
    }

    public static int MaxUpgradeLevelInDeck(List<PlayerCard> cards) =>
        cards.Count == 0 ? 0 : cards.Max(c => c.UpgradeLevel);

    public static List<PlayerCard> AddCardAtLevel(List<PlayerCard> cards, string cardId, int upgradeLevel)
    {
        if (HasCard(cards, cardId)) return cards;
        var result = cards.ToList();
        result.Add(new() { CardId = cardId, UpgradeLevel = upgradeLevel, Upgraded = upgradeLevel > 0 });
        return result;
    }

    public static List<CardDef> RandomCardReward(List<PlayerCard> ownedCards, CardMode mode, int count = 3)
    {
        var pool = CardDefinitions.CardDefs.Where(c => (c.Mode == mode || c.Mode == CardMode.Both) && !HasCard(ownedCards, c.Id)).ToList();
        if (pool.Count == 0) return new();
        var rng = new Random();
        return pool.OrderBy(_ => rng.Next()).Take(Math.Min(count, pool.Count)).ToList();
    }

    public static bool IsDeckValid(List<PlayerCard> cards) => cards.Count >= 4;
    public static int DeckSize(List<PlayerCard> cards) => cards.Count;

    public static List<PlayerCard> Shuffle(List<PlayerCard> arr)
    {
        var a = arr.ToList();
        var rng = new Random();
        for (int i = a.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (a[i], a[j]) = (a[j], a[i]);
        }
        return a;
    }

    public static CardPlayState InitCardPlayState(List<PlayerCard> collection) =>
        new() { Deck = Shuffle(collection.ToList()), Hand = new(), Used = new(), Graveyard = new() };

    public static CardPlayState DrawCards(CardPlayState state, int count = HandSize)
    {
        var deck = state.Deck;
        var graveyard = state.Graveyard;
        if (deck.Count < count && graveyard.Count > 0)
        {
            deck = deck.Concat(Shuffle(graveyard)).ToList();
            graveyard = new();
        }
        var draw = Math.Min(count, deck.Count);
        var drawn = deck.Take(draw).ToList();
        deck = deck.Skip(draw).ToList();
        return new() { Deck = deck, Hand = state.Hand.Concat(drawn).ToList(), Used = state.Used, Graveyard = graveyard };
    }

    public static CardPlayState StartTurn(CardPlayState state)
    {
        var graveyard = state.Graveyard.Concat(state.Hand).Concat(state.Used).ToList();
        var cleared = new CardPlayState { Deck = state.Deck, Hand = new(), Used = new(), Graveyard = graveyard };
        return DrawCards(cleared, HandSize);
    }

    public static CardPlayState? PlayCardFromHand(CardPlayState state, int handIdx)
    {
        if (handIdx < 0 || handIdx >= state.Hand.Count) return null;
        var card = state.Hand[handIdx];
        return new()
        {
            Deck = state.Deck,
            Hand = state.Hand.Where((_, i) => i != handIdx).ToList(),
            Used = state.Used.Append(card).ToList(),
            Graveyard = state.Graveyard,
        };
    }

    public static CardPlayState EndTurn(CardPlayState state) =>
        new() { Deck = state.Deck, Hand = new(), Used = new(), Graveyard = state.Graveyard.Concat(state.Used).Concat(state.Hand).ToList() };
}
