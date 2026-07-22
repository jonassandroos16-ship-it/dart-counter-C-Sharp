namespace dart_counter.Models;

public class BadgeDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Desc { get; set; } = "";
}

public static class Badges
{
    public static readonly List<BadgeDef> All = new()
    {
        new() { Id = "b_first_game", Name = "First Game", Icon = "🎮", Desc = "Played your first game" },
        new() { Id = "b_first_win", Name = "First Win", Icon = "🏆", Desc = "Won your first game" },
        new() { Id = "b_180", Name = "180 Club", Icon = "💥", Desc = "Scored a perfect 180" },
        new() { Id = "b_checkout_100", Name = "Centurion", Icon = "💯", Desc = "Checked out 100+" },
        new() { Id = "b_checkout_170", Name = "Big Fish", Icon = "🐳", Desc = "Checked out 170 (max)" },
        new() { Id = "b_10_games", Name = "Getting Started", Icon = "🎯", Desc = "Played 10 games" },
        new() { Id = "b_50_games", Name = "Dedicated", Icon = "📅", Desc = "Played 50 games" },
        new() { Id = "b_100_games", Name = "Centurion", Icon = "🎖️", Desc = "Played 100 games" },
        new() { Id = "b_5_wins", Name = "Winner", Icon = "🥇", Desc = "Won 5 games" },
        new() { Id = "b_25_wins", Name = "Champion", Icon = "👑", Desc = "Won 25 games" },
        new() { Id = "b_killer", Name = "Killer", Icon = "💀", Desc = "Won a Killer game" },
        new() { Id = "b_battle", Name = "Warrior", Icon = "⚔️", Desc = "Won a Battle game" },
        new() { Id = "b_highscore", Name = "High Roller", Icon = "🎰", Desc = "Won a High Score game" },
        new() { Id = "b_team", Name = "Team Player", Icon = "🤝", Desc = "Won a team match" },
        new() { Id = "b_t20_10", Name = "T20 Striker", Icon = "🎯", Desc = "Hit 10 T20s in a game" },
        new() { Id = "b_bull_5", Name = "Bullseye", Icon = "🐂", Desc = "Hit 5 bullseyes in a game" },
        new() { Id = "b_comeback", Name = "Comeback Kid", Icon = "🔄", Desc = "Won a best-of match" },
    };

    public static BadgeDef? GetInfo(string? id) => All.FirstOrDefault(b => b.Id == id);
}
