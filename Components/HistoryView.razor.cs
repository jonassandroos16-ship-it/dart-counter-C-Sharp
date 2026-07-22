using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;

namespace dart_counter.Components;

public partial class HistoryView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;
    [Inject] ToastService Toast { get; set; } = default!;

    private GameRecord? _detailGame;

    protected override void OnInitialized()
    {
        State.OnChange += StateOnChange;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private void ShowDetail(GameRecord game) => _detailGame = game;
    private void CloseDetail() => _detailGame = null;

    private async Task DeleteGame(GameRecord game)
    {
        State.Games.Remove(game);
        await State.SetGames(State.Games);
        _detailGame = null;
        Toast.Show("Game deleted");
    }
}