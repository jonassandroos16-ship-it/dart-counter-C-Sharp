using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using dart_counter.Services;
using dart_counter.Models;
using dart_counter.Logic;

namespace dart_counter;

public partial class App : ComponentBase, IDisposable
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;
    [Inject] SoundService Sound { get; set; } = default!;
    [Inject] IJSRuntime JS { get; set; } = default!;

    private string _view = "play";
    private bool _navOpen = true;
    private bool _welcomeDone = false;
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        State.OnChange += StateOnChange;
        await State.Initialize();
        var navState = await JS.InvokeAsync<string>("localStorage.getItem", "dc_nav_open");
        _navOpen = navState != "0";
        await JS.InvokeVoidAsync("dartCounter.applyTheme", State.Settings.Theme, State.Settings.Accent);
        _loading = false;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private async Task SetView(string v)
    {
        _view = v;
        await Sound.PlayClick(State.Settings);
    }

    private async Task ToggleNav()
    {
        _navOpen = !_navOpen;
        await JS.InvokeVoidAsync("localStorage.setItem", "dc_nav_open", _navOpen ? "1" : "0");
    }

    private async Task DismissWelcome()
    {
        _welcomeDone = true;
        await JS.InvokeVoidAsync("dartCounter.applyTheme", State.Settings.Theme, State.Settings.Accent);
    }

    public void Dispose()
    {
        State.OnChange -= StateOnChange;
    }

    private record NavItem(string Id, string Label, string Icon);
    private readonly List<NavItem> _nav = new()
    {
        new("play", "Play", "🎯"),
        new("players", "Players", "👥"),
        new("stats", "Stats", "📊"),
        new("history", "History", "📜"),
        new("settings", "Settings", "⚙️"),
    };
}
