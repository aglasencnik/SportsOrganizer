using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace SportsOrganizer.Server.Services;

public class CultureProviderService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CultureProviderService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CultureInfo GetCurrentCultureInfo()
    {
        var defaultCookieName = CookieRequestCultureProvider.DefaultCookieName;

        if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(defaultCookieName, out var cookieValue))
        {
            var requestCulture = CookieRequestCultureProvider.ParseCookieValue(cookieValue);
            return CultureInfo.GetCultureInfo(requestCulture.Cultures[0].ToString());
        }

        return CultureInfo.CurrentCulture;
    }
}
