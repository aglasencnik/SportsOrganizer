using Microsoft.AspNetCore.Components;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Services;

namespace SportsOrganizer.Server.Components.Setup.BasicInfoSetupComponent;

public class BasicInfoSetupBase : ComponentBase
{
    [Parameter]
    public EventCallback<SetupStages> OnSubmit { get; set; }

    [Inject]
    public ApplicationDbContextService DbContextService { get; set; }

    private ApplicationDbContext DbContext => DbContextService.GetDbContext();

    protected override async Task OnInitializedAsync()
    {
        
    }
}
