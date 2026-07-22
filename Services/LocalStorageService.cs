using dart_counter.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace dart_counter.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _js;
    private const string PlayersKey = "dc_players";
    private const string GamesKey = "dc_games";
    private const string SettingsKey = "dc_settings";
    private const string ActiveGameKey = "dc_active_game";

    public LocalStorageService(IJSRuntime js) => _js = js;

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json, JsonOpts);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOpts);
        await _js.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    public async Task RemoveAsync(string key) => await _js.InvokeVoidAsync("localStorage.removeItem", key);

    public async Task<List<Player>> GetPlayers() => await GetAsync<List<Player>>(PlayersKey) ?? new();
    public async Task SetPlayers(List<Player> players) => await SetAsync(PlayersKey, players);

    public async Task<List<GameRecord>> GetGames() => await GetAsync<List<GameRecord>>(GamesKey) ?? new();
    public async Task SetGames(List<GameRecord> games) => await SetAsync(GamesKey, games);

    public async Task<Settings> GetSettings()
    {
        var s = await GetAsync<Settings>(SettingsKey);
        return s ?? new Settings();
    }
    public async Task SetSettings(Settings settings) => await SetAsync(SettingsKey, settings);

    public async Task<Game?> GetActiveGame()
    {
        var g = await GetAsync<Game>(ActiveGameKey);
        if (g == null || g.Finished) return null;
        if (g.ThrownThisRound == null) g.ThrownThisRound = new();
        if (g.Darts == null) g.Darts = new();
        return g;
    }
    public async Task SetActiveGame(Game? game)
    {
        if (game == null) await RemoveAsync(ActiveGameKey);
        else await SetAsync(ActiveGameKey, game);
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
}
