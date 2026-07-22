namespace dart_counter.Models;

public class PowerUpDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Desc { get; set; } = "";
}

public class PowerUpResult
{
    public Game Game { get; set; } = default!;
    public string Message { get; set; } = "";
    public bool Ok { get; set; } = true;
    public bool ShieldBlocked { get; set; }
}

public static class PowerUps
{
    public static readonly string[] AttackIds = { "pu_blocker", "pu_cripple", "pu_steal", "pu_freeze", "pu_swap" };

    public static bool IsAttack(string? id) => id != null && AttackIds.Contains(id);

    public static readonly List<PowerUpDef> All = new()
    {
        new() { Id = "pu_fourth_dart", Name = "Fourth Dart", Icon = "🎯", Desc = "Add a bonus 4th dart to your current visit — score it before tapping Enter Visit." },
        new() { Id = "pu_blocker", Name = "Blocker", Icon = "🛡️", Desc = "Every other player only gets ONE dart on their next visit (instead of three). Blocked by Shield." },
        new() { Id = "pu_reroll", Name = "Reroll", Icon = "🎲", Desc = "Re-roll your lowest-scoring dart this visit. The further behind you are, the luckier the roll — and it will never make you finish." },
        new() { Id = "pu_rethrow", Name = "Re-Throw", Icon = "🔁", Desc = "Take back your last dart this visit and throw it again. Use when you mis-tap a segment." },
        new() { Id = "pu_surge", Name = "Surge", Icon = "⚡", Desc = "Your NEXT visit scores double (activates on your next turn, not this one)." },
        new() { Id = "pu_cripple", Name = "Cripple", Icon = "🦾", Desc = "The leading opponent only scores 50% on their next visit. They are warned when they throw. Blocked by Shield." },
        new() { Id = "pu_steal", Name = "Steal", Icon = "🥷", Desc = "Steal 30 points from the leading opponent. Blocked by Shield." },
        new() { Id = "pu_freeze", Name = "Freeze", Icon = "❄️", Desc = "Freeze the leading opponent — their next visit scores 0 and ends immediately. Blocked by Shield." },
        new() { Id = "pu_lucky_miss", Name = "Lucky Miss", Icon = "🍀", Desc = "Cancel your next bust — if you would bust this visit, your score stays instead." },
        new() { Id = "pu_bullseye_frenzy", Name = "Bullseye Frenzy", Icon = "🐂", Desc = "Your next visit: any dart that hits the bull (25 or 50) scores DOUBLE." },
        new() { Id = "pu_hot_streak", Name = "Hot Streak", Icon = "🔥", Desc = "Your next visit: each dart scores +5 bonus per dart already scored this visit." },
        new() { Id = "pu_swap", Name = "Swap", Icon = "🔄", Desc = "Swap your current score with the leading opponent. Blocked by Shield." },
        new() { Id = "pu_shield", Name = "Shield", Icon = "🏰", Desc = "Shield yourself from all power-up attacks for your next 2 turns." },
    };

    public static PowerUpDef? GetInfo(string? id) => All.FirstOrDefault(p => p.Id == id);

    public static int ChargesNeeded(string? id, Settings settings)
    {
        if (id == null) return settings.PowerUpScaling.ChargeMax;
        if (settings.PowerUpScaling.ChargesNeeded != null && settings.PowerUpScaling.ChargesNeeded.TryGetValue(id, out var v))
            return v;
        return settings.PowerUpScaling.ChargeMax;
    }

    public static int StartingCharge(string? id, Settings settings)
    {
        if (id == null) return 0;
        if (settings.PowerUpScaling.StartingCharge != null && settings.PowerUpScaling.StartingCharge.TryGetValue(id, out var v))
            return v;
        return 0;
    }
}
