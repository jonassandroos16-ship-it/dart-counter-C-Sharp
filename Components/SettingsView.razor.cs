using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;
using Microsoft.JSInterop;
using System.Text.Json;

namespace dart_counter.Components;

public partial class SettingsView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;
    [Inject] SoundService Sound { get; set; } = default!;
    [Inject] IJSRuntime JS { get; set; } = default!;

    protected override void OnInitialized()
    {
        State.OnChange += StateOnChange;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private async Task SetAccent(string color)
    {
        State.Settings.Accent = color;
        await State.SetSettings(State.Settings);
    }

    private async Task TestSound()
    {
        await Sound.PlayHit(State.Settings, 60);
    }

    private async Task SyncNow()
    {
        Toast.Show("Syncing...");
        var (ok, msg) = await State.ManualSync();
        Toast.Show(msg);
    }

    private async Task ExportData()
    {
        var data = new
        {
            players = State.Players,
            games = State.Games,
            settings = State.Settings
        };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        var encoded = Uri.EscapeDataString(json);
        await JS.InvokeVoidAsync("eval", $"navigator.clipboard.writeText(decodeURIComponent('{encoded}'))");
        Toast.Show("Data copied to clipboard!");
    }

    private async Task EraseAllData()
    {
        State.Players.Clear();
        State.Games.Clear();
        State.Settings = new Settings();
        await State.SetPlayers(State.Players);
        await State.SetGames(State.Games);
        await State.SetSettings(State.Settings);
        Toast.Show("All data erased.");
    }
}