using Microsoft.JSInterop;
using System.Text.Json;
using dart_counter.Models;

namespace dart_counter.Services;

public class MultiplayerService
{
    private readonly IJSRuntime _js;
    public string DeviceId { get; private set; } = "";

    public MultiplayerService(IJSRuntime js)
    {
        _js = js;
        _ = InitDeviceId();
    }

    private async Task InitDeviceId()
    {
        try
        {
            DeviceId = await _js.InvokeAsync<string>("dartCounter.getDeviceId") ?? Guid.NewGuid().ToString("N");
        }
        catch { DeviceId = Guid.NewGuid().ToString("N"); }
    }

    public async Task<Lobby?> CreateLobby(string name, Player hostPlayer)
    {
        var code = GenerateLobbyCode();
        var lobby = new Lobby
        {
            Id = Guid.NewGuid().ToString("N"),
            Code = code,
            Name = name,
            HostDeviceId = DeviceId,
            HostPlayerId = hostPlayer.Id,
            Status = "lobby",
            CreatedAt = DateTime.UtcNow.ToString("o"),
            UpdatedAt = DateTime.UtcNow.ToString("o"),
        };
        try
        {
            await _js.InvokeVoidAsync("dartCounter.mpCreateLobby", lobby);
            return lobby;
        }
        catch { return null; }
    }

    public async Task<bool> JoinLobby(string lobbyId, Player player)
    {
        try
        {
            await _js.InvokeVoidAsync("dartCounter.mpJoinLobby", lobbyId, player, DeviceId);
            return true;
        }
        catch { return false; }
    }

    public async Task LeaveLobby(string lobbyId, string playerId)
    {
        try { await _js.InvokeVoidAsync("dartCounter.mpLeaveLobby", lobbyId, DeviceId, playerId); }
        catch { }
    }

    public async Task DeleteLobby(string lobbyId)
    {
        try { await _js.InvokeVoidAsync("dartCounter.mpDeleteLobby", lobbyId); }
        catch { }
    }

    public async Task SetPlayerReady(string lobbyId, string playerId, bool ready)
    {
        try { await _js.InvokeVoidAsync("dartCounter.mpSetReady", lobbyId, DeviceId, playerId, ready); }
        catch { }
    }

    public async Task<Lobby?> FetchLobby(string lobbyId)
    {
        try { return await _js.InvokeAsync<Lobby?>("dartCounter.mpFetchLobby", lobbyId); }
        catch { return null; }
    }

    public async Task<List<LobbyPlayer>> FetchLobbyPlayers(string lobbyId)
    {
        try { return await _js.InvokeAsync<List<LobbyPlayer>>("dartCounter.mpFetchPlayers", lobbyId) ?? new(); }
        catch { return new(); }
    }

    public async Task<List<Lobby>> FetchOpenLobbies()
    {
        try { return await _js.InvokeAsync<List<Lobby>>("dartCounter.mpFetchOpenLobbies") ?? new(); }
        catch { return new(); }
    }

    public async Task<Lobby?> FetchLobbyByCode(string code)
    {
        try { return await _js.InvokeAsync<Lobby?>("dartCounter.mpFetchByCode", code.ToUpperInvariant()); }
        catch { return null; }
    }

    public async Task UpdateGameState(string lobbyId, Game game)
    {
        try { await _js.InvokeVoidAsync("dartCounter.mpUpdateGameState", lobbyId, game); }
        catch { }
    }

    public async Task StartGame(string lobbyId, GameConfig config, Game game)
    {
        try { await _js.InvokeVoidAsync("dartCounter.mpStartGame", lobbyId, config, game); }
        catch { }
    }

    public async Task SetLobbyStatus(string lobbyId, string status)
    {
        try { await _js.InvokeVoidAsync("dartCounter.mpSetStatus", lobbyId, status); }
        catch { }
    }

    public bool IsMyTurn(List<LobbyPlayer> lobbyPlayers, Game? game)
    {
        if (game == null || game.Finished) return false;
        var currentPlayer = game.Players.ElementAtOrDefault(game.CurrentIdx);
        if (currentPlayer == null) return false;
        var lp = lobbyPlayers.FirstOrDefault(p => p.PlayerId == currentPlayer.Id);
        return lp != null && lp.DeviceId == DeviceId;
    }

    public bool OwnsPlayer(List<LobbyPlayer> lobbyPlayers, string playerId) =>
        lobbyPlayers.Any(lp => lp.PlayerId == playerId && lp.DeviceId == DeviceId);

    private static string GenerateLobbyCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rng = new Random();
        return new string(Enumerable.Range(0, 4).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
    }
}
