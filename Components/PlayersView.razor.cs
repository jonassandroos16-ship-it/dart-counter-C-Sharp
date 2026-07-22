using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;

namespace dart_counter.Components;

public partial class PlayersView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;
    [Inject] SoundService Sound { get; set; } = default!;

    private Player? _editingPlayer;
    private bool _isNew = false;
    private string _editTab = "basic";

    protected override void OnInitialized()
    {
        State.OnChange += StateOnChange;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private void AddPlayer()
    {
        _isNew = true;
        _editingPlayer = new Player
        {
            Name = "",
            Color = GameModes.Colors[State.Players.Count % GameModes.Colors.Length],
            Attributes = GameLogic.DefaultAttributes(State.Settings),
            PowerUps = GameLogic.DefaultPowerUps(State.Settings)
        };
        _editTab = "basic";
    }

    private void EditPlayer(Player p)
    {
        _isNew = false;
        _editingPlayer = new Player
        {
            Id = p.Id, Name = p.Name, Color = p.Color,
            Xp = p.Xp, Level = p.Level,
            UnlockedTitles = new(p.UnlockedTitles),
            SelectedTitle = p.SelectedTitle,
            UnlockedBadges = new(p.UnlockedBadges),
            BadgeCounts = p.BadgeCounts,
            SelectedBadge = p.SelectedBadge,
            ShowBadgeContext = p.ShowBadgeContext,
            Sound = p.Sound,
            Attributes = p.Attributes,
            PowerUps = p.PowerUps,
            DeveloperMode = p.DeveloperMode,
            ShowdownBg = p.ShowdownBg
        };
        _editTab = "basic";
    }

    private async Task SavePlayer()
    {
        if (_editingPlayer == null) return;
        if (string.IsNullOrWhiteSpace(_editingPlayer.Name))
        {
            Toast.Show("Player name cannot be empty!");
            return;
        }

        if (_isNew)
        {
            if (_editingPlayer.DeveloperMode)
            {
                _editingPlayer.Xp = 10000;
                var (_, lvl) = GameLogic.LevelFromXp(_editingPlayer.Xp, State.Settings);
                _editingPlayer.Level = lvl;
                _editingPlayer.UnlockedTitles = Titles.Builtin.Select(t => t.Id).ToList();
                _editingPlayer.PowerUps ??= new();
                _editingPlayer.PowerUps.Unlocked = PowerUps.All.Select(p => p.Id).ToList();
                _editingPlayer.UnlockedBadges = Badges.All.Select(b => b.Id).ToList();
            }
            State.Players.Add(_editingPlayer);
        }
        else
        {
            var existing = State.Players.FirstOrDefault(p => p.Id == _editingPlayer.Id);
            if (existing != null)
            {
                existing.Name = _editingPlayer.Name;
                existing.Color = _editingPlayer.Color;
                existing.SelectedTitle = _editingPlayer.SelectedTitle;
                existing.SelectedBadge = _editingPlayer.SelectedBadge;
                existing.Sound = _editingPlayer.Sound;
                existing.ShowdownBg = _editingPlayer.ShowdownBg;
                existing.DeveloperMode = _editingPlayer.DeveloperMode;
                existing.PowerUps = _editingPlayer.PowerUps;
                existing.Attributes = _editingPlayer.Attributes;
            }
        }

        await State.SetPlayers(State.Players);
        _editingPlayer = null;
    }

    private void CloseEdit() => _editingPlayer = null;

    private async Task DeletePlayer(Player p)
    {
        State.Players.Remove(p);
        await State.SetPlayers(State.Players);
        Toast.Show($"Deleted {p.Name}");
    }
}