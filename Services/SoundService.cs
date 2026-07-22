using dart_counter.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dart_counter.Services;

public class SoundService
{
    private readonly IJSRuntime _js;
    public SoundService(IJSRuntime js) => _js = js;

    public async Task PlayClick(Settings settings)
    {
        if (!settings.Sound) return;
        await _js.InvokeVoidAsync("dartCounter.playClick", settings.ClickSound, settings.ClickVolume);
    }

    public async Task PlaySfx(string sfx, Settings settings)
    {
        if (!settings.Sound) return;
        await _js.InvokeVoidAsync("dartCounter.playSfx", sfx, settings.SfxVolume);
    }

    public async Task PlayHit(Settings settings, int score)
    {
        if (!settings.Sound) return;
        await _js.InvokeVoidAsync("dartCounter.playHit", settings.HitSoundPack, settings.SfxVolume, score);
    }

    public async Task PlayEntrance(PlayerSoundId sound, Settings settings)
    {
        if (!settings.Sound || sound == PlayerSoundId.None) return;
        await _js.InvokeVoidAsync("dartCounter.playEntrance", sound.ToString(), settings.SfxVolume);
    }

    public async Task StartMusic(string track, Settings settings)
    {
        if (!settings.Music) return;
        await _js.InvokeVoidAsync("dartCounter.startMusic", track, settings.MusicVolume);
    }

    public async Task StopMusic()
    {
        await _js.InvokeVoidAsync("dartCounter.stopMusic");
    }
}
