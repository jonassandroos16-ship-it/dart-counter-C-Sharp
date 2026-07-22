using dart_counter.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace dart_counter.Services;

public class SupabaseSyncService
{
    private readonly HttpClient _http;
    private const string SupabaseUrl = "https://0ec90b57d6e95fcbda19832f.supabase.co";
    private const string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJib2x0IiwicmVmIjoiMGVjOTBiNTdkNmU5NWZjYmRhMTk4MzJmIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTg4ODE1NzQsImV4cCI6MTc1ODg4MTU3NH0.9I8-U0x86Ak8t2DGaIk0HfvTSLsAyzdnz-Nw00mMkKw";

    public bool HasDatabase => true;
    public bool Connected { get; private set; }
    public bool UpToDate { get; private set; }
    public DateTime? LastSync { get; private set; }
    public bool Syncing { get; private set; }

    public SupabaseSyncService(HttpClient http) => _http = http;

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Add("apikey", SupabaseKey);
        req.Headers.Add("Authorization", $"Bearer {SupabaseKey}");
        return req;
    }

    public async Task<bool> PushAppState(List<Player> players, Settings settings, List<string>? deletedPlayerIds = null, List<string>? deletedGameIds = null)
    {
        try
        {
            var payload = new
            {
                id = "main",
                players,
                settings,
                deleted_player_ids = deletedPlayerIds ?? new List<string>(),
                deleted_game_ids = deletedGameIds ?? new List<string>(),
                updated_at = DateTime.UtcNow
            };
            var req = CreateRequest(HttpMethod.Post, $"{SupabaseUrl}/rest/v1/app_state");
            req.Headers.Add("Prefer", "return=minimal,resolution=merge-duplicates");
            req.Content = JsonContent.Create(payload);
            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> PushGame(GameRecord game)
    {
        try
        {
            var payload = new { id = game.Id, data = game };
            var req = CreateRequest(HttpMethod.Post, $"{SupabaseUrl}/rest/v1/games");
            req.Headers.Add("Prefer", "return=minimal,resolution=merge-duplicates");
            req.Content = JsonContent.Create(payload);
            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteGame(string id)
    {
        try
        {
            var req = CreateRequest(HttpMethod.Delete, $"{SupabaseUrl}/rest/v1/games?id=eq.{id}");
            var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<(List<Player> players, Settings settings, List<GameRecord> games, List<string> deletedPlayerIds, List<string> deletedGameIds)?> PullAll()
    {
        try
        {
            var stateReq = CreateRequest(HttpMethod.Get, $"{SupabaseUrl}/rest/v1/app_state?id=eq.main&select=players,settings,deleted_player_ids,deleted_game_ids");
            var gamesReq = CreateRequest(HttpMethod.Get, $"{SupabaseUrl}/rest/v1/games?select=data");

            var stateResp = await _http.SendAsync(stateReq);
            var gamesResp = await _http.SendAsync(gamesReq);

            List<Player>? players = null;
            Settings? settings = null;
            List<string>? deletedPlayerIds = null;
            List<string>? deletedGameIds = null;
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
                    if (row.TryGetProperty("deleted_player_ids", out var dpEl))
                        deletedPlayerIds = JsonSerializer.Deserialize<List<string>>(dpEl.GetRawText());
                    if (row.TryGetProperty("deleted_game_ids", out var dgEl))
                        deletedGameIds = JsonSerializer.Deserialize<List<string>>(dgEl.GetRawText());
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

            return (players ?? new(), settings ?? new Settings(), games ?? new(), deletedPlayerIds ?? new(), deletedGameIds ?? new());
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

            var okState = await PushAppState(players, settings, pulled.Value.deletedPlayerIds, pulled.Value.deletedGameIds);
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
