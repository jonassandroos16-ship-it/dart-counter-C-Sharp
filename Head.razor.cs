using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using dart_counter.Services;

namespace dart_counter;

public partial class Head : ComponentBase
{
    [Inject] GameStateService State { get; set; } = default!;

    protected override void OnInitialized() => State.OnChange += StateOnChange;

    private void StateOnChange() => InvokeAsync(StateHasChanged);

    public void Dispose() => State.OnChange -= StateOnChange;
}
