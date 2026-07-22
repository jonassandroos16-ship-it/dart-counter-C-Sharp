using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;

namespace dart_counter.Components;

public partial class PlayersView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;

    private Player? _editing;
    private bool _isNew;
    private string _editName = "";
    private string _editColor = "#22c55e";

    private void AddPlayer()
    {
        var newPlayer = new Player
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = "",
            Color = GameModes.Colors[State.Players.Count % GameModes.Colors.Length],
            Xp = 0,
            Level = 0,
            UnlockedTitles = new(),
            SelectedTitle = null,
            UnlockedBadges = new(),
            BadgeCounts = new(),
            SelectedBadge = null,
            Attributes = GameLogic.DefaultAttributes(State.Settings),
            PowerUps = GameLogic.DefaultPowerUps(State.Settings)
        };
        State.Players.Add(newPlayer);
        _ = State.SetPlayers(State.Players);
        _editing = newPlayer;
        _isNew = true;
        _editName = "";
        _editColor = newPlayer.Color;
    }

    private void EditPlayer(Player p)
    {
        _editing = p;
        _isNew = false;
        _editName = p.Name;
        _editColor = p.Color;
    }

    private async Task SavePlayer()
    {
        if (_editing == null) return;
        if (string.IsNullOrWhiteSpace(_editName))
        {
            if (_isNew)
            {
                State.Players.Remove(_editing);
                await State.SetPlayers(State.Players);
            }
            _editing = null;
            return;
        }

        _editing.Name = _editName.Trim();
        _editing.Color = _editColor;
        await State.SetPlayers(State.Players);
        Toast.Show($"Player saved");
        _editing = null;
    }

    private void CancelEdit()
    {
        if (_editing != null && _isNew)
        {
            State.Players.Remove(_editing);
            _ = State.SetPlayers(State.Players);
        }
        _editing = null;
    }

    private async Task DeletePlayer(Player p)
    {
        if (!await JSConfirm($"Delete {p.Name}?")) return;
        State.Players.Remove(p);
        await State.SetPlayers(State.Players);
        Toast.Show("Player deleted");
    }

    [Inject] IJSRuntime JS { get; set; } = default!;
    private async Task<bool> JSConfirm(string msg) => await JS.InvokeAsync<bool>("confirm", msg);
}
