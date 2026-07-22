namespace dart_counter.Models;

public static class CampaignChapters
{
    public static readonly List<CampaignChapter> All = new()
    {
        new()
        {
            Id = "crimson_vale",
            Name = "Chapter I · The Crimson Vale",
            Subtitle = "Break the warlord Malakar",
            Theme = new() { Id = "crimson", Name = "Crimson", Background = "radial-gradient(circle at 50% 0%, color-mix(in srgb,#7f1d1d 45%, var(--bg)) 0%, var(--bg) 70%)", Accent = "#ef4444", CardTint = "color-mix(in srgb,#ef4444 12%, var(--bg-2))" },
            Intro = "Warlord Malakar has tightened his grip on the Crimson Vale. Villages burn, roads are choked with raiders, and the few who escape speak of a crimson citadel rising over the hills. The party rides east — darts sharpened, hearts steady — to break his hold before the vale falls entirely.",
            Outro = "The citadel gates crash inward. Malakar falls, his crimson banner torn beneath your boots. The vale draws its first free breath in years. But as the dust settles, a cold wind sweeps down from the north — and with it, whispers of a frozen throne waking beneath the ice.",
            Levels = new()
            {
                new() { LevelId = 1, Name = "Peasant's Outpost", Enemies = new() { "goblin_scout", "goblin_scout" }, RewardPowerUp = "coop_meteor", StoryBit = "The outpost falls quiet. The first of Malakar's eyes is closed. We move on." },
                new() { LevelId = 2, Name = "Goblin Camp", Enemies = new() { "goblin_scout", "goblin_brute" }, RewardPowerUp = "coop_phantom", StoryBit = "Smoke rises from the camp behind us. The road to the citadel is open." },
                new() { LevelId = 3, Name = "Forest Ambush", Enemies = new() { "goblin_brute", "goblin_brute", "goblin_scout" }, RewardPowerUp = "coop_time_warp", StoryBit = "The trees go still. Whatever was waiting in the dark is waiting no longer." },
                new() { LevelId = 4, Name = "Raider Crossing", Enemies = new() { "orc_raider", "goblin_scout" }, RewardPowerUp = "coop_ressurect", StoryBit = "The crossing is ours. The citadel looms above the ridge — one more push." },
                new() { LevelId = 5, Name = "The Crimson Citadel", IsBoss = true, Enemies = new() { "orc_raider", "warlord_malakar" }, RewardPowerUp = "coop_apocalypse", StoryBit = "Malakar kneels in the rubble of his own throne. The Crimson Vale is free." },
            }
        },
        new()
        {
            Id = "frozen_throne",
            Name = "Chapter II · The Frozen Throne",
            Subtitle = "Silence the Ice Court",
            Theme = new() { Id = "ice", Name = "Ice", Background = "radial-gradient(circle at 50% 0%, color-mix(in srgb,#1e3a8a 45%, var(--bg)) 0%, var(--bg) 70%)", Accent = "#60a5fa", CardTint = "color-mix(in srgb,#60a5fa 14%, var(--bg-2))" },
            Intro = "The cold wind from the north carried more than ash — it carried a name. The Ice Court has woken beneath the glacier, and its throne is empty no longer. Frost creeps southward, rivers freeze mid-current, and the villagers of the vale whisper of a pale figure walking the snowfields. The party wraps their cloaks tight and climbs into the white.",
            Outro = "The throne cracks down the middle and the long winter breaks with it. Ice gives way to water, water to green. As the party descends the thawing pass, a distant roar rolls up from the south — thick, wet, alive. The jungle has noticed the cold is gone, and it is hungry.",
            Levels = new()
            {
                new() { LevelId = 1, Name = "Frostfang Pass", Enemies = new() { "ice_wolf", "ice_wolf" }, RewardPowerUp = "coop_blizzard", StoryBit = "The wolves scatter into the snow. The pass is open — for now." },
                new() { LevelId = 2, Name = "Glacier Outpost", Enemies = new() { "frost_archer", "ice_wolf" }, RewardPowerUp = "coop_frostbite", StoryBit = "The outpost's fires are out. We press on before the cold finds us again." },
                new() { LevelId = 3, Name = "The Hollow Cavern", Enemies = new() { "frost_archer", "frost_archer", "ice_wolf" }, RewardPowerUp = "coop_ice_lance", StoryBit = "The cavern's echoes fade. Whatever lived down here lives here no longer." },
                new() { LevelId = 4, Name = "Throne Approach", Enemies = new() { "frost_knight", "frost_archer" }, RewardPowerUp = "coop_winter_veil", StoryBit = "The throne room is close. The air is too still to be empty." },
                new() { LevelId = 5, Name = "The Frozen Throne", IsBoss = true, Enemies = new() { "frost_knight", "ice_queen" }, RewardPowerUp = "coop_glacial_doom", StoryBit = "The throne splits. The long winter ends, and the south begins to thaw." },
            }
        },
        new()
        {
            Id = "verdant_maw",
            Name = "Chapter III · The Verdant Maw",
            Subtitle = "Silence the living jungle",
            Theme = new() { Id = "jungle", Name = "Jungle", Background = "radial-gradient(circle at 50% 0%, color-mix(in srgb,#14532d 45%, var(--bg)) 0%, var(--bg) 70%)", Accent = "#22c55e", CardTint = "color-mix(in srgb,#22c55e 14%, var(--bg-2))" },
            Intro = "The long winter broke, and the south woke up wet and green and wrong. The jungle that the Ice Court had held frozen for a century is growing back angrier than anyone remembers — vines across the roads, roots through the temple stones, and a low wet roar that rolls up from the canopy at night. The party wraps their cloaks tight against thorns instead of cold now, and pushes south into the green. Something at the heart of the Maw is calling the wild in, and it has to be answered.",
            Outro = "The Heart of the Maw splits like a rotten fruit, and the jungle goes quiet — the kind of quiet that follows a long-held breath. The vines slacken, the roots still, and for the first time in living memory the road south is just a road. The party turns for home, cloaks torn and quivers near empty, but the darts that ride in them now carry the weight of three fallen thrones.",
            Levels = new()
            {
                new() { LevelId = 1, Name = "Thornwood Edge", Enemies = new() { "vine_lasher", "vine_lasher" }, RewardPowerUp = "coop_vine_grasp", StoryBit = "The vines go slack at the treeline. The Maw has noticed us — and we have noticed it back." },
                new() { LevelId = 2, Name = "Spore Hollow", Enemies = new() { "spore_bloom", "vine_lasher" }, RewardPowerUp = "coop_spore_burst", StoryBit = "The hollow is still. The air is thick with something sweet and bad." },
                new() { LevelId = 3, Name = "The Hanging Garden", Enemies = new() { "spore_bloom", "thorn_spearman", "vine_lasher" }, RewardPowerUp = "coop_thorn_lance", StoryBit = "The garden falls. Whatever it was grown to feed is close now." },
                new() { LevelId = 4, Name = "Maw Approach", Enemies = new() { "thorn_spearman", "bloom_warden" }, RewardPowerUp = "coop_verdant_bloom", StoryBit = "The canopy parts. The Maw is open ahead, and it is breathing." },
                new() { LevelId = 5, Name = "The Heart of the Maw", IsBoss = true, Enemies = new() { "bloom_warden", "the_verdant_maw" }, RewardPowerUp = "coop_heart_of_maw", StoryBit = "The Heart splits like rotten fruit. The long green grip on the south is broken at last." },
            }
        },
    };

    public static CampaignChapter? GetChapter(string id) => All.FirstOrDefault(c => c.Id == id);
    public static CampaignChapter? GetChapterByIndex(int idx) => idx >= 0 && idx < All.Count ? All[idx] : null;

    public static int ChapterLevelCount(string chapterId) => GetChapter(chapterId)?.Levels.Count ?? 0;

    public static bool IsChapterComplete(string chapterId, PlayerCampaignProgress? progress)
    {
        var cleared = progress?.Chapters?.GetValueOrDefault(chapterId) ?? 0;
        return cleared >= ChapterLevelCount(chapterId);
    }

    public static bool IsChapterUnlocked(int chapterIndex, PlayerCampaignProgress? progress)
    {
        if (chapterIndex <= 0) return true;
        var prev = All[chapterIndex - 1];
        return prev != null && IsChapterComplete(prev.Id, progress);
    }
}
