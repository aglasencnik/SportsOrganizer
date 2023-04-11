using Microsoft.AspNetCore.Components;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Components.Setup.BasicInfoSetupComponent;

public class BasicInfoSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }
}
