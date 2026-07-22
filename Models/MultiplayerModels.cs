namespace dart_counter.Models;

public class LobbyPlayer
{
    public string Id { get; set; } = "";
    public string LobbyId { get; set; } = "";
    public string DeviceId { get; set; } = "";
    public string PlayerId { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string PlayerColor { get; set; } = "#22c55e";
    public bool Ready { get; set; }
    public string JoinedAt { get; set; } = "";
}

public class GameConfig
{
    public string Mode { get; set; } = "501";
    public bool DoubleOut { get; set; }
    public int Legs { get; set; } = 3;
    public bool TeamMode { get; set; }
    public int[]? TeamAssignment { get; set; }
    public bool PowerUps { get; set; }
}

public class Lobby
{
    public string Id { get; set; } = "";
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string HostDeviceId { get; set; } = "";
    public string HostPlayerId { get; set; } = "";
    public string Status { get; set; } = "lobby";
    public GameConfig? GameConfig { get; set; }
    public Game? GameState { get; set; }
    public int PlayerTurn { get; set; }
    public string CreatedAt { get; set; } = "";
    public string UpdatedAt { get; set; } = "";
}

public class LobbyWithPlayers
{
    public Lobby Lobby { get; set; } = new();
    public List<LobbyPlayer> Players { get; set; } = new();
}
