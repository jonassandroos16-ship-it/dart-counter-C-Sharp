using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;
using System.Text.Json;

namespace dart_counter.Components;

public partial class SettingsView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;
    [Inject] IJSRuntime JS { get; set; } = default!;

    private bool _showExport = false;
    private string _exportData = "";

    protected override void OnInitialized() => State.OnChange += StateOnChange;
    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private async Task UpdateSettings(Settings settings) => await State.SetSettings(settings);

    private void Update(Action<Settings> fn)
    {
        fn(State.Settings);
        _ = State.SetSettings(State.Settings);
    }

    private async Task ToggleTheme()
    {
        State.Settings.Theme = State.Settings.Theme == "dark" ? "light" : "dark";
        await State.SetSettings(State.Settings);
        await JS.InvokeVoidAsync("dartCounter.applyTheme", State.Settings.Theme, State.Settings.Acccent);
    }

    private async Task SetAccent(string color)
    {
        State.Settings.Acccent = color;
        await State.SetSettings(State.Settings);
        await JS.InvokeVoidAsync("dartCounter.applyTheme", State.Settings.Theme, State.Settings.Acccent);
    }

    private async Task ExportData()
    {
        var data = new { players = State.Players, games = State.Games, settings = State.Settings, exportedAt = DateTime.UtcNow };
        _exportData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        _showExport = true;
        await Task.CompletedTask;
    }

    private async Task Sync()
    {
        var (ok, msg) = await State.ManualSync();
        Toast.Show(msg);
    }
}
