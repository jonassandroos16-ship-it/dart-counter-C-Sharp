using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;
using dart_counter.Components;

namespace dart_counter;

public partial class App : ComponentBase, IDisposable
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;
    [Inject] SoundService Sound { get; set; } = default!;

    private bool _showWelcome = true;
    private bool _welcomeGone = false;
    private bool _loading = true;
    private string _view = "play";
    private bool _navOpen = false;

    private PopupInfo? _popup;
    private LevelUpInfo? _levelUp;
    private TitleUnlockInfo? _titleUnlock;
    private KillInfo? _kill;

    protected override async Task OnInitialized()
    {
        State.OnChange += StateOnChange;
        Toast.OnChange += ToastOnChange;
        await State.Initialize();
        _loading = false;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);
    private void ToastOnChange() => InvokeAsync(StateHasChanged);

    private void DismissWelcome()
    {
        if (_welcomeGone) return;
        _welcomeGone = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(700);
            _showWelcome = false;
            await InvokeAsync(StateHasChanged);
        });
    }

    private void SwitchView(string view)
    {
        _view = view;
        _navOpen = false;
        State.Notify();
    }

    private void ToggleNav() => _navOpen = !_navOpen;

    public void ShowPopup(string emoji, string title, string sub, bool record = false)
    {
        _popup = new PopupInfo { Emoji = emoji, Title = title, Sub = sub, Record = record };
        _ = Sound.PlaySfx("milestone", State.Settings);
        _ = AutoClose(() => _popup = null, 2500);
        State.Notify();
    }

    public void ShowLevelUp(int level, string name, int xpGained, string reason)
    {
        _levelUp = new LevelUpInfo { Level = level, Name = name, XpGained = xpGained, Reason = reason };
        _ = Sound.PlaySfx("levelup", State.Settings);
        _ = AutoClose(() => _levelUp = null, 3000);
        State.Notify();
    }

    public void ShowTitleUnlock(string icon, string name, string player, string desc)
    {
        _titleUnlock = new TitleUnlockInfo { Icon = icon, Name = name, Player = player, Desc = desc };
        _ = Sound.PlaySfx("title", State.Settings);
        _ = AutoClose(() => _titleUnlock = null, 3500);
        State.Notify();
    }

    public void ShowKill(string killer, string victim)
    {
        _kill = new KillInfo { Killer = killer, Victim = victim };
        _ = Sound.PlaySfx("kill", State.Settings);
        _ = AutoClose(() => _kill = null, 2200);
        State.Notify();
    }

    private async Task AutoClose(Action close, int ms)
    {
        await Task.Delay(ms);
        close();
        await InvokeAsync(StateHasChanged);
    }

    private void ClosePopup() { _popup = null; State.Notify(); }
    private void CloseLevelUp() { _levelUp = null; State.Notify(); }
    private void CloseTitleUnlock() { _titleUnlock = null; State.Notify(); }
    private void CloseKill() { _kill = null; State.Notify(); }

    public void Dispose()
    {
        State.OnChange -= StateOnChange;
        Toast.OnChange -= ToastOnChange;
    }
}

public class PopupInfo { public string Emoji { get; set; } = ""; public string Title { get; set; } = ""; public string Sub { get; set; } = ""; public bool Record { get; set; } }
public class LevelUpInfo { public int Level { get; set; } public string Name { get; set; } = ""; public int XpGained { get; set; } public string Reason { get; set; } = ""; }
public class TitleUnlockInfo { public string? Icon { get; set; } public string Name { get; set; } = ""; public string Player { get; set; } = ""; public string Desc { get; set; } = ""; }
public class KillInfo { public string Killer { get; set; } = ""; public string Victim { get; set; } = ""; }