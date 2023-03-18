using Ecommerce.Client.BackendClient;
using Ecommerce.Contracts.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ecommerce.Client.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public LoginRequest loginRequest { get; set; } = null!;

    public void OnGet()
    {
    }

    public IActionResult OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);

        
        // var authResponse = await _backendHttpClient.LoginAsync(loginRequest);

        // var claimsPrincipal = GetClaimsPrincipal(authResponse.AccessToken);

        // Response.Cookies.Append("access-token", authResponse.AccessToken);
        // Response.Cookies.Append("refresh-token", authResponse.RefreshToken);

        // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return RedirectToPage("/Index");
    }

    private ClaimsPrincipal GetClaimsPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = tokenHandler.ReadJwtToken(token).Claims;
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }
}