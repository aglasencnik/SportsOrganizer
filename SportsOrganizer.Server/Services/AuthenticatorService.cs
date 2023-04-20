using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Newtonsoft.Json;
using SportsOrganizer.Data;
using SportsOrganizer.Data.Models;
using System.Security.Claims;

namespace SportsOrganizer.Server.Services;

public class AuthenticatorService : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private readonly ApplicationDbContextService _dataProviderService;
    private ApplicationDbContext _dataProvider;

    public AuthenticatorService(ProtectedLocalStorage protectedLocalStorage, ApplicationDbContextService dataProviderService)
    {
        _protectedLocalStorage = protectedLocalStorage;
        _dataProviderService = dataProviderService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal();

        try
        {
            var storedPrincipal = await _protectedLocalStorage.GetAsync<string>("identity");

            if (storedPrincipal.Success)
            {
                var user = JsonConvert.DeserializeObject<UserModel>(storedPrincipal.Value);
                var (_, isLookUpSuccess) = LookUpUser(user.Username, user.Password);

                if (isLookUpSuccess)
                {
                    var identity = CreateIdentityFromUser(user);
                    principal = new(identity);
                }
            }
        }
        catch
        {

        }

        return new AuthenticationState(principal);
    }

    public async Task LoginAsync(string username, string password)
    {
        var (userInDatabase, isSuccess) = LookUpUser(username, password);
        var principal = new ClaimsPrincipal();

        if (isSuccess)
        {
            var identity = CreateIdentityFromUser(userInDatabase);
            principal = new ClaimsPrincipal(identity);
            await _protectedLocalStorage.SetAsync("identity", JsonConvert.SerializeObject(userInDatabase));
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task LogoutAsync()
    {
        await _protectedLocalStorage.DeleteAsync("identity");
        var principal = new ClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    private ClaimsIdentity CreateIdentityFromUser(UserModel user)
    {
        return new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.Name, user.Username),
                new (ClaimTypes.Hash, user.Password),
                new (ClaimTypes.Role, user.UserType.ToString())
            }, "SportsOrganizer");
    }

    private (UserModel, bool) LookUpUser(string username, string password)
    {
        _dataProvider = _dataProviderService.GetDbContext();
        var result = _dataProvider.Users.FirstOrDefault(u => username == u.Username && password == u.Password);

        return (result, result is not null);
    }
}
