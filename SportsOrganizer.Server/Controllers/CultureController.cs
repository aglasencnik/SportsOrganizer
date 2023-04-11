using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SportsOrganizer.Server.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult Set(string culture, string redirectUri)
    {
        if (culture is not null)
        {
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new(culture, culture)));
        }

        return LocalRedirect(redirectUri);
    }
}