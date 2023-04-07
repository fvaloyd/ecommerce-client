using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ecommerce.Client.Services;

namespace Ecommerce.Client.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly IEcommerceApi _ecommerceApi;

    public LoginModel(IEcommerceApi ecommerceApi)
    {
        _ecommerceApi = ecommerceApi;
    }

    [BindProperty]
    public LoginRequest LoginRequest { get; set; } = null!;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var authResponse = await _ecommerceApi.LoginUser(LoginRequest);

        var claimsPrincipal = GetClaimsPrincipalFromToken(authResponse.AccessToken);

        SetTheTokensCookies(authResponse);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return RedirectToPage("/Home/Index");
    }

    private static ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
    {
        IEnumerable<Claim> claims = GetClaimsFromToken(token);
        ClaimsIdentity claimsIdentity = GetClaimsIdentityFromClaims(claims);
        return new ClaimsPrincipal(claimsIdentity);
    }

    private static ClaimsIdentity GetClaimsIdentityFromClaims(IEnumerable<Claim> claims)
        => new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    private static IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = tokenHandler.ReadJwtToken(token).Claims;
        return claims;
    }

    private void SetTheTokensCookies(AuthenticateResponse authResponse)
    {
        Response.Cookies.Append("access-token", authResponse.AccessToken);
        Response.Cookies.Append("refresh-token", authResponse.RefreshToken);
    }
}