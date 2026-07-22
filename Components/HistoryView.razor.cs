using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;

namespace dart_counter.Components;

public partial class HistoryView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;

    private GameRecord? _detail;

    protected override void OnInitialized() => State.OnChange += StateOnChange;
    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private async Task DeleteGame(GameRecord g)
    {
        State.Games.Remove(g);
        await State.SetGames(State.Games);
        _detail = null;
        Toast.Show("Game deleted");
    }
}
