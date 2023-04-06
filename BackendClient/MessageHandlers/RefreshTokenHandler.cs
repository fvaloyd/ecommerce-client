using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Ecommerce.Client.BackendClient.MessageHandlers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public RefreshTokenHandler(IHttpClientFactory httpFactory, IHttpContextAccessor contextAccessor)
    {
        _httpClientFactory = httpFactory;
        _contextAccessor = contextAccessor;
    }

    const string EcommerceApiClientName = "EcommerceApi";
    const string RefreshTokenApiPath = "/api/Token/refresh";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is not HttpStatusCode.Unauthorized)
            return response;

        var httpClient = _httpClientFactory.CreateClient(EcommerceApiClientName);
        RefreshTokenRequest refreshTokenRequest = GetRefreshTokenRequestContent(request);
        var refreshTokenResponse = await httpClient.PostAsJsonAsync(RefreshTokenApiPath, refreshTokenRequest);
        if (!refreshTokenResponse.IsSuccessStatusCode)
            return response;

        var authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(await refreshTokenResponse.Content.ReadAsStringAsync());
        
        RemoveExpireTokenCookies();
        SetRefreshedTokenCookies(authResponse!);

        // Set the authorization header with the refreshed token
        request.Headers.Authorization = new AuthenticationHeaderValue(
                                                JwtBearerDefaults.AuthenticationScheme,
                                                authResponse!.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }

    private void SetRefreshedTokenCookies(AuthenticateResponse authResponse)
    {
        _contextAccessor.HttpContext!.Response.Cookies.Append("access-token", authResponse!.AccessToken);
        _contextAccessor.HttpContext!.Response.Cookies.Append("refresh-token", authResponse!.RefreshToken);
    }

    private void RemoveExpireTokenCookies()
    {
        _contextAccessor.HttpContext!.Response.Cookies.Delete("refresh-token");
        _contextAccessor.HttpContext!.Response.Cookies.Delete("access-token");
    }

    private RefreshTokenRequest GetRefreshTokenRequestContent(HttpRequestMessage req)
    {
        string? accessToken = req.Headers.Authorization!.Parameter;
        string refreshTokenEncoded = req.Headers.FirstOrDefault(h => h.Key == "refresh-token").Value.First();
        string refreshToken = WebUtility.UrlDecode(refreshTokenEncoded).Replace(" ", "+");
        
        return new RefreshTokenRequest(accessToken!, refreshToken);
    }
}


