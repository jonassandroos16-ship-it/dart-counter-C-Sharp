using dart_counter.Models;

namespace dart_counter.Logic;

public static class GameExtensions
{
    public static bool HighScore(this Game game) => game.Mode == "highscore";
}
