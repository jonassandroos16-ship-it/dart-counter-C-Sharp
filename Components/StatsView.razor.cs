using Microsoft.AspNetCore.Components;
using dart_counter.Models;
using dart_counter.Logic;
using dart_counter.Services;

namespace dart_counter.Components;

public partial class StatsView : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;

    private string? _selectedPlayerId;

    protected override void OnInitialized()
    {
        State.OnChange += StateOnChange;
        if (State.Players.Count > 0) _selectedPlayerId = State.Players[0].Id;
    }

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    private Player? SelectedPlayer => State.Players.FirstOrDefault(p => p.Id == _selectedPlayerId);
}
