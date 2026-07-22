using dart_counter.Models;
using Microsoft.AspNetCore.Components;

namespace dart_counter.Services;

public class SoundService
{
    private readonly IJSRuntime _js;
    public SoundService(IJSRuntime js) => _js = js;

    public async Task PlayClick(Settings settings)
    {
        if (!settings.Sound) return;
        await _js.InvokeVoidAsync("dartCounter.playClick", settings.ClickVolume);
    }

    public async Task PlaySfx(string sfx, Settings settings)
    {
        if (!settings.Sound) return;
        await _js.InvokeVoidAsync("dartCounter.playSfx", sfx, settings.SfxVolume);
    }
}
