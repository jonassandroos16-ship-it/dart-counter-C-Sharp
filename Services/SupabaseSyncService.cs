using dart_counter.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace dart_counter.Services;

public class SupabaseSyncService
{
    private readonly HttpClient _http;
    private const string SupabaseUrl = "https://tcamuctvnwqjpgraiqzw.supabase.co";
    private const string SupabaseKey = "sb_publishable_a2c7SoWIAH5attOpTJQtDQ_PsEiGC9X";

    public bool HasDatabase => true;
    public bool Connected { get; private set; }
    public bool UpToDate { get; private set; }
    public DateTime? LastSync { get; private set; }
    public bool Syncing { get; private set; }

    public SupabaseSyncService(HttpClient http) => _http = http;

    private void SetHeaders()
    {
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Add("apikey", SupabaseKey);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}");
    }

    public async Task<bool> PushAppState(List<Player> players, Settings settings)
    {
        try
        {
            SetHeaders();
            var payload = new
            {
                id = "main",
                players,
                settings,
                updated_at = DateTime.UtcNow
            };
            var resp = await _http.PutAsJsonAsync($"{SupabaseUrl}/rest/v1/app_state?id=eq.main", payload);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> PushGame(GameRecord game)
    {
        try
        {
            SetHeaders();
            var payload = new { id = game.Id, data = game };
            var resp = await _http.PutAsJsonAsync($"{SupabaseUrl}/rest/v1/games?id=eq.{game.Id}", payload);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteGame(string id)
    {
        try
        {
            SetHeaders();
            var resp = await _http.DeleteAsync($"{SupabaseUrl}/rest/v1/games?id=eq.{id}");
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<(List<Player> players, Settings settings, List<GameRecord> games)?> PullAll()
    {
        try
        {
            SetHeaders();
            var stateResp = await _http.GetAsync($"{SupabaseUrl}/rest/v1/app_state?id=eq.main&select=players,settings");
            var gamesResp = await _http.GetAsync($"{SupabaseUrl}/rest/v1/games?select=data");

            List<Player>? players = null;
            Settings? settings = null;
            List<GameRecord>? games = null;

            if (stateResp.IsSuccessStatusCode)
            {
                var stateJson = await stateResp.Content.ReadAsStringAsync();
                var stateArr = JsonSerializer.Deserialize<JsonElement[]>(stateJson);
                if (stateArr != null && stateArr.Length > 0)
                {
                    var row = stateArr[0];
                    if (row.TryGetProperty("players", out var pEl))
                        players = JsonSerializer.Deserialize<List<Player>>(pEl.GetRawText());
                    if (row.TryGetProperty("settings", out var sEl))
                        settings = JsonSerializer.Deserialize<Settings>(sEl.GetRawText());
                }
            }

            if (gamesResp.IsSuccessStatusCode)
            {
                var gamesJson = await gamesResp.Content.ReadAsStringAsync();
                var gamesArr = JsonSerializer.Deserialize<JsonElement[]>(gamesJson);
                if (gamesArr != null)
                {
                    games = new();
                    foreach (var el in gamesArr)
                    {
                        if (el.TryGetProperty("data", out var dataEl))
                        {
                            var game = JsonSerializer.Deserialize<GameRecord>(dataEl.GetRawText());
                            if (game != null) games.Add(game);
                        }
                    }
                }
            }

            return (players ?? new(), settings ?? new Settings(), games ?? new());
        }
        catch { return null; }
    }

    public async Task<(bool ok, string message)> ManualSync(List<Player> players, Settings settings, List<GameRecord> games)
    {
        Syncing = true;
        try
        {
            var pulled = await PullAll();
            if (pulled == null)
            {
                Connected = false;
                return (false, "Could not reach server.");
            }

            Connected = true;

            var okState = await PushAppState(players, settings);
            var okGames = true;
            foreach (var g in games)
            {
                if (!await PushGame(g)) okGames = false;
            }

            UpToDate = okState && okGames;
            LastSync = DateTime.UtcNow;
            return (true, UpToDate ? "Synced." : "Partial sync.");
        }
        catch (Exception ex)
        {
            Connected = false;
            return (false, ex.Message);
        }
        finally { Syncing = false; }
    }
}
