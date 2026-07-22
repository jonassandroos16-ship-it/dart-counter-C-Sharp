namespace dart_counter.Logic;

using dart_counter.Models;

public static class CoopShields
{
    static readonly int[] DartboardOrder = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };

    public static int[] NeighborsOf(int baseNum)
    {
        var i = Array.IndexOf(DartboardOrder, baseNum);
        if (i < 0) return Array.Empty<int>();
        var left = DartboardOrder[(i - 1 + DartboardOrder.Length) % DartboardOrder.Length];
        var right = DartboardOrder[(i + 1) % DartboardOrder.Length];
        return new[] { left, right };
    }

    public static bool IsTopHalf(int baseNum)
    {
        var i = Array.IndexOf(DartboardOrder, baseNum);
        return i >= 0 && (i < 5 || i > 15);
    }

    public static bool IsBottomHalf(int baseNum)
    {
        var i = Array.IndexOf(DartboardOrder, baseNum);
        return i >= 6 && i <= 14;
    }

    public static bool IsLeftHalf(int baseNum)
    {
        var i = Array.IndexOf(DartboardOrder, baseNum);
        return i >= 11;
    }

    public static bool IsRightHalf(int baseNum)
    {
        var i = Array.IndexOf(DartboardOrder, baseNum);
        return i >= 1 && i <= 9;
    }

    public static bool DartMatchesShield(CampaignDart dart, ShieldLayer shield)
    {
        if (shield.Type == ShieldType.Span)
        {
            if (dart.Base == 0) return false;
            var span = shield.Span;
            if (span == null) return false;
            return span switch
            {
                SpanTarget.TopHalf => IsTopHalf(dart.Base),
                SpanTarget.BottomHalf => IsBottomHalf(dart.Base),
                SpanTarget.LeftHalf => IsLeftHalf(dart.Base),
                SpanTarget.RightHalf => IsRightHalf(dart.Base),
                SpanTarget.AnyDouble => dart.IsDouble,
                SpanTarget.AnyTriple => dart.Mult == 3 && !dart.IsBull,
                SpanTarget.AnyBull => dart.Base == 25 || dart.Base == 50,
                _ => false
            };
        }
        return MatchesExactTarget(dart, shield.TargetValue);
    }

    public static bool MatchesExactTarget(CampaignDart dart, string t)
    {
        if (t == "Bull") return dart.Base == 50;
        if (t == "25") return dart.Base == 25 && !dart.IsBull;
        var m = System.Text.RegularExpressions.Regex.Match(t, @"^([DT]?)(\d+)$");
        if (!m.Success) return false;
        var mult = m.Groups[1].Value == "D" ? 2 : m.Groups[1].Value == "T" ? 3 : 1;
        if (!int.TryParse(m.Groups[2].Value, out var baseNum)) return false;
        if (dart.Base != baseNum) return false;
        if (baseNum == 25 || baseNum == 50) return true;
        return dart.Mult == mult;
    }

    public static string DescribeShield(ShieldLayer shield)
    {
        if (shield.Type == ShieldType.Span)
        {
            var span = shield.Span;
            return span switch
            {
                SpanTarget.TopHalf => "Top Half",
                SpanTarget.BottomHalf => "Bottom Half",
                SpanTarget.LeftHalf => "Left Half",
                SpanTarget.RightHalf => "Right Half",
                SpanTarget.AnyDouble => "Any Double",
                SpanTarget.AnyTriple => "Any Triple",
                SpanTarget.AnyBull => "Any Bull",
                _ => shield.TargetValue
            };
        }
        var t = shield.TargetValue;
        if (t == "Bull") return "Bullseye";
        if (t == "25") return "25 (outer bull)";
        var m = System.Text.RegularExpressions.Regex.Match(t, @"^([DT]?)(\d+)$");
        if (!m.Success) return t;
        var prefix = m.Groups[1].Value == "D" ? "Double " : m.Groups[1].Value == "T" ? "Triple " : "Single ";
        return prefix + m.Groups[2].Value;
    }
}
