using dart_counter.Models;

namespace dart_counter.Logic;

public static class GameExtensions
{
    public static bool HighScore(this Game game) => game.Mode == "highscore";
    public static bool IsBattle(this Game game) => game.Mode == "battle";
    public static bool IsKiller(this Game game) => game.Mode == "killer";
    public static bool IsSpeed(this Game game) => game.Mode == "speed101";
}
