using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UOrders.WebUI.AuthProviders;

public class AuthStateProvider : AuthenticationStateProvider
{
    #region Private Fields

    private readonly AuthenticationState _anonymous;
    private readonly ILocalStorageService _localStorage;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly Timer _timer;

    #endregion Private Fields

    #region Public Constructors

    public AuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        _tokenHandler = new();
        _timer = new(
            callback: new TimerCallback(BackgroundTokenCheck),
            state: null,
            dueTime: new TimeSpan(0, 1, 0),
            period: new TimeSpan(0, 1, 0)
        );
    }

    #endregion Public Constructors

    #region Public Methods

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwtToken = await GetValidToken();

        if (jwtToken is null)
        {
            await Logout();
            return _anonymous;
        }
        else
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwtAuthType")));
    }

    public async Task Login(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);
        var jwtToken = _tokenHandler.ReadJwtToken(token);
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwtAuthType"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        var authState = Task.FromResult(_anonymous);
        NotifyAuthenticationStateChanged(authState);
    }

    #endregion Public Methods

    #region Private Methods

    private void BackgroundTokenCheck(object? state)
    {
        _ = GetAuthenticationStateAsync();
    }

    private async Task<JwtSecurityToken?> GetValidToken()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrEmpty(token)) return null;

        var jwtToken = _tokenHandler.ReadJwtToken(token);

        if (jwtToken is null) return null;
        if (jwtToken.ValidTo <= DateTime.UtcNow) return null;

        return jwtToken;
    }

    #endregion Private Methods
}