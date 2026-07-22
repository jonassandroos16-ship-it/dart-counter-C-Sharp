using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;
using Microsoft.AspNetCore.Components;

namespace dart_counter;

public class GameStateService
{
    public List<Player> Players { get; set; } = new();
    public List<GameRecord> Games { get; set; } = new();
    public Settings Settings { get; set; } = new();
    public Game? ActiveGame { get; set; }
    public bool HasDatabase => true;
    public bool Connected { get; set; }
    public bool UpToDate { get; set; }
    public DateTime? LastSync { get; set; }
    public bool Syncing { get; set; }

    public event Action? OnChange;

    private readonly LocalStorageService _localStorage;
    private readonly SupabaseSyncService _sync;
    private bool _initialized = false;

    public GameStateService(LocalStorageService localStorage, SupabaseSyncService sync)
    {
        _localStorage = localStorage;
        _sync = sync;
    }

    public async Task Initialize()
    {
        if (_initialized) return;
        _initialized = true;
        try
        {
            Players = await _localStorage.GetPlayers();
            Games = await _localStorage.GetGames();
            Settings = await _localStorage.GetSettings();
            ActiveGame = await _localStorage.GetActiveGame();
        }
        catch { }
        Notify();

        _ = BackgroundSync();
    }

    private async Task BackgroundSync()
    {
        try
        {
            var pulled = await _sync.PullAll();
            if (pulled == null) { Connected = false; return; }

            Connected = true;
            var remote = pulled.Value;

            var localPlayerIds = new HashSet<string>(Players.Select(p => p.Id));
            foreach (var rp in remote.players)
                if (!localPlayerIds.Contains(rp.Id))
                    Players.Add(rp);

            var localGameIds = new HashSet<string>(Games.Select(g => g.Id));
            foreach (var rg in remote.games)
                if (!localGameIds.Contains(rg.Id))
                    Games.Add(rg);

            await _localStorage.SetPlayers(Players);
            await _localStorage.SetGames(Games);
            UpToDate = true;
            LastSync = DateTime.UtcNow;
            Notify();
        }
        catch { Connected = false; }
    }

    public async Task SetPlayers(List<Player> players)
    {
        Players = players;
        await _localStorage.SetPlayers(players);
        Notify();
    }

    public async Task SetGames(List<GameRecord> games)
    {
        Games = games;
        await _localStorage.SetGames(games);
        Notify();
    }

    public async Task SetSettings(Settings settings)
    {
        Settings = settings;
        await _localStorage.SetSettings(settings);
        Notify();
    }

    public async Task SetActiveGame(Game? game)
    {
        ActiveGame = game;
        await _localStorage.SetActiveGame(game);
        Notify();
    }

    public async Task<(bool ok, string msg)> ManualSync()
    {
        var result = await _sync.ManualSync(Players, Settings, Games);
        Connected = _sync.Connected;
        UpToDate = _sync.UpToDate;
        LastSync = _sync.LastSync;
        Syncing = _sync.Syncing;
        Notify();
        return result;
    }

    public void Notify() => OnChange?.Invoke();
}
