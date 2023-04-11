using Microsoft.AspNetCore.Components;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Components.Setup.ThemeSetupComponent;

public class ThemeSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }
}
