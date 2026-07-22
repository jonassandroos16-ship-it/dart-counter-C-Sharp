using dart_counter.Models;

namespace dart_counter.Services;

public class ToastService
{
    public string? Message { get; private set; }
    public bool Visible => !string.IsNullOrEmpty(Message);
    public event Action? OnChange;
    private CancellationTokenSource? _cts;

    public void Show(string msg)
    {
        Message = msg;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _ = HideAfterDelay(_cts.Token);
        OnChange?.Invoke();
    }

    private async Task HideAfterDelay(CancellationToken ct)
    {
        try { await Task.Delay(1800, ct); Message = null; OnChange?.Invoke(); }
        catch { }
    }
}
