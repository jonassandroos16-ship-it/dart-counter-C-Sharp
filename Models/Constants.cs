namespace dart_counter.Models;

public static class GameModes
{
    public class ModeDef
    {
        public string Key { get; set; } = "";
        public int Start { get; set; }
        public string Label { get; set; } = "";
        public bool Atc { get; set; }
        public bool Practice { get; set; }
        public bool Killer { get; set; }
        public bool Party { get; set; }
        public string Desc { get; set; } = "";
        public List<string> Rules { get; set; } = new();
    }

    public static readonly Dictionary<string, ModeDef> Modes = new()
    {
        ["501"] = new ModeDef { Key = "501", Start = 501, Label = "501", Desc = "The classic. Race from 501 down to zero.", Rules = new() { "Each player starts on 501.", "Take turns throwing 3 darts per visit.", "Subtract each visit from your remaining score.", "First to reach exactly zero wins the leg.", "Optionally require a double to finish (Double Out).", "Best-of multiple legs decides the match." } },
        ["301"] = new ModeDef { Key = "301", Start = 301, Label = "301", Desc = "A shorter version of 501 — faster finishes.", Rules = new() { "Each player starts on 301.", "Take turns throwing 3 darts per visit.", "Subtract each visit from your remaining score.", "First to reach exactly zero wins the leg.", "Optionally require a double to finish (Double Out).", "Best-of multiple legs decides the match." } },
        ["701"] = new ModeDef { Key = "701", Start = 701, Label = "701", Desc = "A longer format for more players or extended matches.", Rules = new() { "Each player starts on 701.", "Take turns throwing 3 darts per visit.", "Subtract each visit from your remaining score.", "First to reach exactly zero wins the leg.", "Optionally require a double to finish (Double Out).", "Best-of multiple legs decides the match." } },
        ["101"] = new ModeDef { Key = "101", Start = 101, Label = "101", Desc = "A quick sprint — one visit can win it.", Rules = new() { "Each player starts on 101.", "Take turns throwing 3 darts per visit.", "Subtract each visit from your remaining score.", "First to reach exactly zero wins the leg.", "Optionally require a double to finish (Double Out).", "Best-of multiple legs decides the match." } },
        ["atc"] = new ModeDef { Key = "atc", Start = 0, Label = "Around the Clock", Atc = true, Desc = "Hit the numbers 1 through 20 and the Bull in order.", Rules = new() { "Hit numbers 1, 2, 3 … 20, then the Bull — in sequence.", "Any segment (single, double, triple) counts.", "3 darts per visit, then play passes to the next player.", "First to land the Bull wins.", "Great for practicing the whole board." } },
        ["practice"] = new ModeDef { Key = "practice", Start = 0, Label = "Practice", Practice = true, Desc = "Free scoring — no rules, just throw.", Rules = new() { "No target score and no checkout.", "Throw 3 darts per visit and watch your totals stack up.", "Turns rotate between players so everyone can warm up.", "Use it to groove your throw or test combinations.", "Practice games do not count toward competitive stats." } },
        ["killer"] = new ModeDef { Key = "killer", Start = 0, Label = "Killer", Killer = true, Desc = "Hit your number 5 times to become a Killer, then knock out opponents by hitting their number.", Rules = new() { "Each player is assigned a number on the board.", "Hit your own number 5 times to become a Killer.", "Once a Killer, hit opponents' numbers to remove their lives.", "Each player starts with 3 lives.", "Lose all your lives and you're eliminated.", "Last player standing wins." } },
        ["speed101"] = new ModeDef { Key = "speed101", Start = 101, Label = "Speed 101", Party = true, Desc = "Race to zero from 101 — fastest checkout wins. No double-out.", Rules = new() { "Everyone starts on 101.", "Race to reach exactly zero.", "Straight Out only — no double required.", "First to check out wins the round.", "A party mode built for quick, chaotic fun." } },
        ["highscore"] = new ModeDef { Key = "highscore", Start = 0, Label = "High Score", Party = true, Desc = "Score as much as you can in 7 visits. Highest total wins!", Rules = new() { "No target score — pure scoring.", "Each player gets exactly 7 visits of 3 darts.", "Your total score across all 7 visits is your result.", "Highest total wins.", "A party mode for finding out who can really pile on the points." } },
        ["battle"] = new ModeDef { Key = "battle", Start = 0, Label = "Battle", Party = true, Desc = "Use your attributes! Each dart deals damage to an opponent. Last one standing wins.", Rules = new() { "Each player starts with HP equal to their Health attribute (base 300, cap 500).", "Armor is a flat reduction applied to EVERY dart (base 0, cap 25).", "Power is a flat bonus added to EVERY dart that hits (base 0, cap 30).", "Each visit you attack one opponent — in 1v1 it is automatic.", "Per-dart damage = (dart value + power) − armor, min 1 on a hit.", "Misses deal 0 damage (power only applies on successful hits).", "Reduce an opponent to 0 HP to defeat them.", "Last player standing wins the battle." } },
    };

    public static List<string> ModeKeys => Modes.Keys.ToList();

    public static readonly int[] AtcTargets = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, -1 };
    public static string AtcLabel(int i) => i >= AtcTargets.Length ? "Done" : (AtcTargets[i] == -1 ? "Bull" : AtcTargets[i].ToString());

    public static readonly int[] DartboardNumbers = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    public static readonly int[] KillerNumbers = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    public static readonly string[] Colors = { "#22c55e", "#3b82f6", "#f59e0b", "#ef4444", "#06b6d4", "#ec4899", "#a855f7", "#84cc16" };
    public static readonly string[] TeamColors = { "#3b82f6", "#ef4444", "#22c55e", "#f59e0b" };
    public static readonly string[] TeamNames = { "Alpha", "Bravo", "Delta", "Echo" };
}

public static class Checkouts
{
    public static readonly Dictionary<int, string[]> Table = new()
    {
        [170] = new[] { "T20", "T20", "Bull" }, [167] = new[] { "T20", "T19", "Bull" }, [164] = new[] { "T20", "T18", "Bull" }, [161] = new[] { "T20", "T17", "Bull" },
        [160] = new[] { "T20", "T20", "D20" }, [158] = new[] { "T20", "T20", "D19" }, [157] = new[] { "T20", "T19", "D20" }, [156] = new[] { "T20", "T20", "D18" },
        [155] = new[] { "T20", "T19", "D19" }, [154] = new[] { "T20", "T18", "D20" }, [153] = new[] { "T20", "T19", "D18" }, [152] = new[] { "T20", "T20", "D16" },
        [151] = new[] { "T20", "T17", "D20" }, [150] = new[] { "T20", "T18", "D18" }, [149] = new[] { "T20", "T19", "D16" }, [148] = new[] { "T20", "T20", "D14" },
        [147] = new[] { "T20", "T17", "D18" }, [146] = new[] { "T20", "T18", "D16" }, [145] = new[] { "T20", "T19", "D14" }, [144] = new[] { "T20", "T20", "D12" },
        [143] = new[] { "T20", "T17", "D16" }, [142] = new[] { "T20", "T14", "D20" }, [141] = new[] { "T20", "T19", "D12" }, [140] = new[] { "T20", "T20", "D10" },
        [139] = new[] { "T20", "T13", "D20" }, [138] = new[] { "T20", "T18", "D12" }, [137] = new[] { "T20", "T19", "D10" }, [136] = new[] { "T20", "T20", "D8" },
        [135] = new[] { "Bull", "T15", "D20" }, [134] = new[] { "T20", "T14", "D16" }, [133] = new[] { "T20", "T19", "D8" }, [132] = new[] { "Bull", "T14", "D20" },
        [131] = new[] { "T20", "T13", "D16" }, [130] = new[] { "T20", "T18", "D8" }, [129] = new[] { "T19", "T16", "D12" }, [128] = new[] { "T18", "T14", "D16" },
        [127] = new[] { "T20", "T17", "D8" }, [126] = new[] { "T19", "T19", "D6" }, [125] = new[] { "Bull", "T20", "D8" }, [124] = new[] { "T20", "T16", "D8" },
        [123] = new[] { "T19", "T16", "D9" }, [122] = new[] { "T18", "T20", "D4" }, [121] = new[] { "T20", "T11", "D14" }, [120] = new[] { "T20", "20", "D20" },
        [110] = new[] { "T20", "10", "D20" }, [107] = new[] { "T19", "10", "D20" }, [104] = new[] { "T18", "10", "D20" }, [100] = new[] { "T20", "D20" },
        [98] = new[] { "T20", "D19" }, [97] = new[] { "T19", "D20" }, [96] = new[] { "T20", "D18" }, [95] = new[] { "T19", "D19" }, [94] = new[] { "T18", "D20" },
        [92] = new[] { "T20", "D16" }, [90] = new[] { "T20", "D15" }, [89] = new[] { "T19", "D16" }, [86] = new[] { "T18", "D16" }, [84] = new[] { "T20", "D12" },
        [81] = new[] { "T19", "D12" }, [80] = new[] { "T20", "D10" }, [76] = new[] { "T20", "D8" }, [72] = new[] { "T16", "D12" }, [70] = new[] { "T18", "D8" },
        [68] = new[] { "T20", "D4" }, [67] = new[] { "T17", "D8" }, [64] = new[] { "T16", "D8" }, [61] = new[] { "T15", "D8" }, [60] = new[] { "20", "D20" },
        [58] = new[] { "18", "D20" }, [57] = new[] { "17", "D20" }, [56] = new[] { "16", "D20" }, [54] = new[] { "14", "D20" }, [52] = new[] { "12", "D20" },
        [50] = new[] { "Bull" }, [48] = new[] { "16", "D16" }, [46] = new[] { "6", "D20" }, [44] = new[] { "12", "D16" }, [42] = new[] { "10", "D16" }, [40] = new[] { "D20" },
        [38] = new[] { "D19" }, [36] = new[] { "D18" }, [34] = new[] { "D17" }, [32] = new[] { "D16" }, [30] = new[] { "D15" }, [28] = new[] { "D14" }, [26] = new[] { "D13" }, [24] = new[] { "D12" },
        [22] = new[] { "D11" }, [20] = new[] { "D10" }, [18] = new[] { "D9" }, [16] = new[] { "D8" }, [14] = new[] { "D7" }, [12] = new[] { "D6" }, [10] = new[] { "D5" }, [8] = new[] { "D4" }, [6] = new[] { "D3" }, [4] = new[] { "D2" }, [2] = new[] { "D1" },
    };
}

public static class ScorePopups
{
    public class PopupDef { public int Min { get; set; } public string Emoji { get; set; } = ""; public string Title { get; set; } = ""; public string Sub { get; set; } = ""; }

    public static readonly List<PopupDef> Scores = new()
    {
        new() { Min = 180, Emoji = "💥", Title = "ONE HUNDRED AND EIGHTY!", Sub = "Maximum score!" },
        new() { Min = 140, Emoji = "🔥", Title = "Big Score!", Sub = "140+ with 3 darts" },
        new() { Min = 120, Emoji = "⚡", Title = "Ton+", Sub = "120+ with 3 darts" },
        new() { Min = 100, Emoji = "💯", Title = "Ton!", Sub = "100+ with 3 darts" },
        new() { Min = 80, Emoji = "🎯", Title = "80+", Sub = "Solid visit" },
        new() { Min = 60, Emoji = "👍", Title = "60+", Sub = "Steady scoring" },
    };

    public static readonly List<PopupDef> Milestones = new()
    {
        new() { Min = 200, Emoji = "🎯", Title = "Below 200!", Sub = "On the finish" },
        new() { Min = 150, Emoji = "🔥", Title = "Below 150!", Sub = "Checkout range" },
        new() { Min = 100, Emoji = "⚡", Title = "Below 100!", Sub = "Closing in" },
    };
}

public static class ShowdownBgs
{
    public class BgDef { public string Id { get; set; } = ""; public string Label { get; set; } = ""; public string Css { get; set; } = ""; }

    public static readonly List<BgDef> All = new()
    {
        new() { Id = "default", Label = "Midnight", Css = "radial-gradient(circle at 50% 40%, #1a1d2a 0%, #0a0c12 70%)" },
        new() { Id = "inferno", Label = "Inferno", Css = "radial-gradient(circle at 50% 30%, #4a0e0e 0%, #1a0606 60%, #0a0303 100%)" },
        new() { Id = "aurora", Label = "Aurora", Css = "linear-gradient(135deg, #0a1a2a 0%, #0d3b2e 40%, #1a4a3a 70%, #06121a 100%)" },
        new() { Id = "voltage", Label = "Voltage", Css = "radial-gradient(circle at 30% 40%, #1a2a4a 0%, #0a1430 50%, #050818 100%)" },
        new() { Id = "sunset", Label = "Sunset", Css = "linear-gradient(180deg, #2a1a3a 0%, #4a2a3a 40%, #6a3a2a 70%, #1a0a1a 100%)" },
        new() { Id = "glacier", Label = "Glacier", Css = "radial-gradient(circle at 50% 40%, #1a3a4a 0%, #0a1a2a 60%, #050a14 100%)" },
        new() { Id = "neon", Label = "Neon Grid", Css = "linear-gradient(180deg, #1a0a2a 0%, #2a0a3a 50%, #0a0518 100%)" },
        new() { Id = "forest", Label = "Deep Forest", Css = "radial-gradient(circle at 50% 50%, #0e2a1a 0%, #06180e 60%, #030a06 100%)" },
        new() { Id = "crimson", Label = "Crimson", Css = "radial-gradient(circle at 50% 30%, #3a0a1a 0%, #1a050a 60%, #0a0205 100%)" },
        new() { Id = "storm", Label = "Storm", Css = "linear-gradient(180deg, #1a1a2a 0%, #2a2a3a 40%, #0a0a1a 100%)" },
    };

    public static string CssFor(string? id)
    {
        if (string.IsNullOrEmpty(id)) return All[0].Css;
        var found = All.FirstOrDefault(b => b.Id == id);
        return found?.Css ?? All[0].Css;
    }
}
