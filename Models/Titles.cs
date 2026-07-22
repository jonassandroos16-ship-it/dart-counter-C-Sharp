namespace dart_counter.Models;

public class TitleDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Desc { get; set; } = "";
    public string Icon { get; set; } = "";
    public bool Custom { get; set; }
}

public class TitleCtx
{
    public string? PlayerId { get; set; }
    public List<GameRecord> Games { get; set; } = new();
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public List<Visit> LifetimeVisits { get; set; } = new();
}

public static class Titles
{
    public static readonly List<TitleDef> Builtin = new()
    {
        new() { Id = "t20king", Name = "Triple 20 King", Desc = "Hit T20 five times in one game", Icon = "👑" },
        new() { Id = "t20trio", Name = "T20 Trio", Desc = "Hit T20 three times in one game", Icon = "🎯" },
        new() { Id = "t20marathon", Name = "T20 Marathon", Desc = "Hit T20 ten times in one game", Icon = "🏃" },
        new() { Id = "t1master", Name = "Triple 1 Master", Desc = "Hit T1 three times in one game", Icon = "🎪" },
        new() { Id = "cow_tipper", Name = "Cow Tipper", Desc = "Hit a single bull (25) in one game", Icon = "🐄" },
        new() { Id = "milkman", Name = "The Milkman", Desc = "Hit 3 single bulls (25) in one game", Icon = "🥛" },
        new() { Id = "cattle_rustler", Name = "Cattle Rustler", Desc = "Hit 5 bulls-or-outer in one game", Icon = "🐮" },
        new() { Id = "first_bull", Name = "First Blood", Desc = "Hit your first bullseye (50) in a game", Icon = "🎯" },
        new() { Id = "bullseye", Name = "Bullseye Hunter", Desc = "Hit 3 bullseyes (50) in one game", Icon = "🐂" },
        new() { Id = "bullseye_pro", Name = "Bullseye Pro", Desc = "Hit 5 bullseyes (50) in one game", Icon = "🎖️" },
        new() { Id = "ton80", Name = "Ton 80 Hero", Desc = "Score a 180", Icon = "💥" },
        new() { Id = "double_vision", Name = "Double Vision", Desc = "Score two 180s in one game", Icon = "👀" },
        new() { Id = "ton_machine", Name = "Ton Machine", Desc = "Hit 3+ tons (100+) in one game", Icon = "💯" },
        new() { Id = "ton_factory", Name = "Ton Factory", Desc = "Hit 6+ tons (100+) in one game", Icon = "🏭" },
        new() { Id = "big_hitter", Name = "Big Hitter", Desc = "Score 140+ three times in one game", Icon = "🔥" },
        new() { Id = "consistent", Name = "The Consistent", Desc = "Score 60+ in 5 consecutive visits", Icon = "📊" },
        new() { Id = "checkout_1", Name = "Baby Checkout", Desc = "Checkout with less than 10", Icon = "🍼" },
        new() { Id = "checkout_10", Name = "Tiny Tim", Desc = "Checkout with 10+", Icon = "🧸" },
        new() { Id = "checkout_25", Name = "Pocket Change", Desc = "Checkout with 25+", Icon = "🪙" },
        new() { Id = "checkout_40", Name = "Two Darts Wonder", Desc = "Checkout with 40+", Icon = "🪄" },
        new() { Id = "checkout_60", Name = "Steady Eddie", Desc = "Checkout with 60+", Icon = "🧱" },
        new() { Id = "checkout_80", Name = "Mid-Pack Mike", Desc = "Checkout with 80+", Icon = "🧭" },
        new() { Id = "checkout_king", Name = "Checkout King", Desc = "Checkout with 100+", Icon = "🎯" },
        new() { Id = "big_finish", Name = "Big Finish", Desc = "Checkout with 120+", Icon = "🎆" },
        new() { Id = "high_ton_out", Name = "High Ton Out", Desc = "Checkout with 150+", Icon = "🏆" },
        new() { Id = "checkout_170", Name = "Big Fish", Desc = "Checkout with 170 (the max)", Icon = "🐳" },
        new() { Id = "sharpshooter", Name = "Sharpshooter", Desc = "Hit 10+ triples in one game", Icon = "🔫" },
        new() { Id = "triple_stack", Name = "Triple Stack", Desc = "Hit 3 triples in one visit", Icon = "🥞" },
        new() { Id = "double_dip", Name = "Double Dip", Desc = "Hit 3 doubles in one visit", Icon = "💠" },
        new() { Id = "double_trouble", Name = "Double Trouble", Desc = "Bust 3 times in one game", Icon = "😵" },
        new() { Id = "comeback_kid", Name = "Comeback Kid", Desc = "Win a best-of match", Icon = "🔄" },
        new() { Id = "first9_flyer", Name = "First 9 Flyer", Desc = "Average 40+ in first 9 darts", Icon = "🚀" },
        // Lifetime titles
        new() { Id = "life_score_1k", Name = "Rookie Scorer", Desc = "Score 1,000 points across all games", Icon = "🥉" },
        new() { Id = "life_score_5k", Name = "Regular Scorer", Desc = "Score 5,000 points across all games", Icon = "🥈" },
        new() { Id = "life_score_10k", Name = "Serious Scorer", Desc = "Score 10,000 points across all games", Icon = "🥇" },
        new() { Id = "life_score_25k", Name = "Score Grinder", Desc = "Score 25,000 points across all games", Icon = "⚔️" },
        new() { Id = "life_score_50k", Name = "Score Legend", Desc = "Score 50,000 points across all games", Icon = "🌟" },
        new() { Id = "life_score_100k", Name = "Score Titan", Desc = "Score 100,000 points across all games", Icon = "💎" },
        new() { Id = "life_t20_25", Name = "T20 Hunter", Desc = "Hit 25 triple 20s in a lifetime", Icon = "🎯" },
        new() { Id = "life_t20_100", Name = "T20 Sniper", Desc = "Hit 100 triple 20s in a lifetime", Icon = "🏹" },
        new() { Id = "life_t20_500", Name = "T20 Machine", Desc = "Hit 500 triple 20s in a lifetime", Icon = "🤖" },
        new() { Id = "life_180_1", Name = "First 180", Desc = "Score your first 180", Icon = "💥" },
        new() { Id = "life_180_10", Name = "180 Collector", Desc = "Score ten 180s in a lifetime", Icon = "🧨" },
        new() { Id = "life_180_25", Name = "180 Addict", Desc = "Score twenty-five 180s in a lifetime", Icon = "🎰" },
        new() { Id = "life_180_50", Name = "180 Maestro", Desc = "Score fifty 180s in a lifetime", Icon = "🎼" },
        new() { Id = "life_bulls_10", Name = "Bull Wrangler", Desc = "Hit 10 bullseyes (50) in a lifetime", Icon = "🐃" },
        new() { Id = "life_bulls_50", Name = "Bull Master", Desc = "Hit 50 bullseyes (50) in a lifetime", Icon = "👑" },
        new() { Id = "life_bulls_75", Name = "Bull Whisperer", Desc = "Hit 75 bullseyes (50) in a lifetime", Icon = "🤫" },
        new() { Id = "life_bulls_100", Name = "Bull Legend", Desc = "Hit 100 bullseyes (50) in a lifetime", Icon = "🏅" },
        new() { Id = "life_triples_50", Name = "Triple Threat", Desc = "Hit 50 triples in a lifetime", Icon = "3️⃣" },
        new() { Id = "life_triples_500", Name = "Triple Titan", Desc = "Hit 500 triples in a lifetime", Icon = "🔱" },
        new() { Id = "life_doubles_50", Name = "Double Down", Desc = "Hit 50 doubles in a lifetime", Icon = "2️⃣" },
        new() { Id = "life_doubles_500", Name = "Double Dynasty", Desc = "Hit 500 doubles in a lifetime", Icon = "♊" },
        new() { Id = "life_checkouts_10", Name = "Finisher", Desc = "Complete 10 checkouts in a lifetime", Icon = "✅" },
        new() { Id = "life_checkouts_50", Name = "The Closer", Desc = "Complete 50 checkouts in a lifetime", Icon = "🚪" },
        new() { Id = "life_checkouts_100", Name = "Checkout Machine", Desc = "Complete 100 checkouts in a lifetime", Icon = "🏭" },
        new() { Id = "life_checkouts_500", Name = "The Terminator", Desc = "Complete 500 checkouts in a lifetime", Icon = "🤖" },
        new() { Id = "life_games_10", Name = "Getting Started", Desc = "Play 10 games", Icon = "🎮" },
        new() { Id = "life_games_50", Name = "Regular Player", Desc = "Play 50 games", Icon = "📅" },
        new() { Id = "life_games_100", Name = "Half Centurion", Desc = "Play 100 games", Icon = "💯" },
        new() { Id = "life_games_250", Name = "Veteran", Desc = "Play 250 games", Icon = "🎖️" },
        new() { Id = "life_games_500", Name = "Dart Hermit", Desc = "Play 500 games", Icon = "🧙" },
        new() { Id = "life_wins_1", Name = "First Win", Desc = "Win your first game", Icon = "🏆" },
        new() { Id = "life_wins_5", Name = "Winner", Desc = "Win 5 games", Icon = "🏆" },
        new() { Id = "life_wins_10", Name = "Double Digits", Desc = "Win 10 games", Icon = "🥈" },
        new() { Id = "life_wins_25", Name = "Champion", Desc = "Win 25 games", Icon = "🏆" },
        new() { Id = "life_wins_50", Name = "Dominator", Desc = "Win 50 games", Icon = "👑" },
        new() { Id = "life_wins_100", Name = "Dart Legend", Desc = "Win 100 games", Icon = "🌠" },
        // Killer
        new() { Id = "killer_first", Name = "First Kill", Desc = "Eliminate a player in Killer", Icon = "💀" },
        new() { Id = "killer_3", Name = "Killing Spree", Desc = "Eliminate 3 players in one Killer game", Icon = "🔫" },
        new() { Id = "killer_survivor", Name = "Last One Standing", Desc = "Win a Killer game", Icon = "🥇" },
        new() { Id = "killer_marked", Name = "Marked for Death", Desc = "Become a Killer (5 hits on your number)", Icon = "🔪" },
        new() { Id = "killer_flawless", Name = "Flawless Victory", Desc = "Win Killer without losing a life", Icon = "✨" },
        // Party
        new() { Id = "speed_demon", Name = "Speed Demon", Desc = "Win a Speed 101 game", Icon = "⚡" },
        new() { Id = "speed_blur", Name = "Blur", Desc = "Checkout 101 in a single visit", Icon = "💨" },
        new() { Id = "high_roller", Name = "High Roller", Desc = "Win a High Score game", Icon = "🎰" },
        new() { Id = "jackpot", Name = "Jackpot", Desc = "Score 400+ in a High Score game", Icon = "💰" },
        new() { Id = "party_animal", Name = "Party Animal", Desc = "Play 5 party-mode games", Icon = "🎉" },
        // Battle
        new() { Id = "battle_first_win", Name = "First Blood", Desc = "Win your first Battle match", Icon = "🩸" },
        new() { Id = "battle_5_wins", Name = "Brawler", Desc = "Win 5 Battle matches", Icon = "🥊" },
        new() { Id = "battle_25_wins", Name = "Warlord", Desc = "Win 25 Battle matches", Icon = "⚔️" },
        new() { Id = "battle_titan", Name = "Battle Titan", Desc = "Win 100 Battle matches", Icon = "🌋" },
        new() { Id = "battle_knockout", Name = "Knockout", Desc = "Defeat an opponent in a Battle match", Icon = "💀" },
        new() { Id = "battle_flawless", Name = "Flawless Victory", Desc = "Win a Battle match without dropping below half HP", Icon = "✨" },
        new() { Id = "battle_3v1", Name = "Last Stand", Desc = "Win a Battle match against 3+ opponents", Icon = "🏰" },
        new() { Id = "battle_bruiser", Name = "Bruiser", Desc = "Deal 500+ total damage in a single Battle match", Icon = "💥" },
        new() { Id = "battle_ironhide", Name = "Ironhide", Desc = "Have max armor equipped for a Battle match", Icon = "🛡️" },
        new() { Id = "battle_powerhouse", Name = "Powerhouse", Desc = "Have max power equipped for a Battle match", Icon = "⚡" },
        // Team
        new() { Id = "team_first_win", Name = "Team Player", Desc = "Win your first team match", Icon = "🤝" },
        new() { Id = "team_5_wins", Name = "Squad Up", Desc = "Win 5 team matches", Icon = "👥" },
        new() { Id = "team_25_wins", Name = "Dream Team", Desc = "Win 25 team matches", Icon = "🌟" },
        new() { Id = "team_4stack", Name = "Four Stack", Desc = "Win a team match with 4 teams playing", Icon = "🟦" },
        new() { Id = "team_clutch", Name = "Clutch Team", Desc = "Win a best-of team match (3+ legs)", Icon = "🛡️" },
        new() { Id = "team_rivalry", Name = "Rivalry", Desc = "Play a team match against at least 2 other teams", Icon = "⚔️" },
    };

    public static List<TitleDef> All(List<CustomTitle> custom) => Builtin.Concat(custom.Select(t => new TitleDef
    {
        Id = t.Id, Name = t.Name, Desc = t.Desc, Icon = t.Icon, Custom = true
    })).ToList();

    public static TitleDef? GetInfo(string? id, List<CustomTitle> custom) =>
        All(custom).FirstOrDefault(t => t.Id == id);
}
