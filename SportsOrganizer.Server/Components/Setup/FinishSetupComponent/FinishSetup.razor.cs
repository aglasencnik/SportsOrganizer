using Microsoft.AspNetCore.Components;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Components.Setup.FinishSetupComponent;

public class FinishSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }
}
