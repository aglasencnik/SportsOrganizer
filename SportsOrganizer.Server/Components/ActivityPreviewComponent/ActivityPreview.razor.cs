using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SportsOrganizer.Data.Models;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Components.ActivityPreviewComponent;

public class ActivityPreviewBase : ComponentBase
{
    [Parameter] 
    public ActivityPreviewType ActivityPreviewType { get; set; }

    [Parameter]
    public ActivityModel Activity { get; set; }

    [Inject]
    protected IStringLocalizer<ActivityPreview> Localizer { get; set; }
}
