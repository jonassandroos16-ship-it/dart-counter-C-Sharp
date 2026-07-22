using Microsoft.JSInterop;

namespace dart_counter.Services;

public class MusicService
{
    private readonly IJSRuntime _js;
    public string CurrentTrack { get; private set; } = "";
    public string CurrentContext { get; private set; } = "";

    public MusicService(IJSRuntime js) => _js = js;

    public async Task Start(string track, Settings settings)
    {
        if (!settings.Music) return;
        CurrentTrack = track;
        CurrentContext = track;
        await _js.InvokeVoidAsync("dartCounter.startMusic", track, settings.MusicVolume);
    }

    public async Task Stop()
    {
        CurrentTrack = "";
        CurrentContext = "";
        await _js.InvokeVoidAsync("dartCounter.stopMusic");
    }

    public async Task SwitchTo(string track, Settings settings)
    {
        if (track == CurrentTrack) return;
        await Stop();
        await Start(track, settings);
    }

    public async Task PlayStart(Settings settings) => await Start(settings.MusicStartTrack, settings);
    public async Task PlaySetup(Settings settings) => await SwitchTo(settings.MusicSetupTrack, settings);
    public async Task PlayMatch(Settings settings) => await SwitchTo(settings.MusicMatchTrack, settings);
    public async Task PlayCoop(Settings settings) => await SwitchTo(settings.MusicCoopTrack, settings);
}
