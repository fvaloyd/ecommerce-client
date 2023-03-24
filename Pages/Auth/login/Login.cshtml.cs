using Ecommerce.Contracts.Endpoints;
using Ecommerce.Client.BackendClient;
using Ecommerce.Contracts.Authentication;

using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.Cookies;

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

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);

        var response = await client.PostAsJsonAsync<LoginRequest>(AuthEndpoints.Login, loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            return Page();
        }

        var responseReaded = await response.Content.ReadAsStringAsync();

        var authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(responseReaded);

        var claimsPrincipal = GetClaimsPrincipal(authResponse!.AccessToken);

        Response.Cookies.Append("access-token", authResponse.AccessToken);
        Response.Cookies.Append("refresh-token", authResponse.RefreshToken);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

         return RedirectToPage("/Home/Index");
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