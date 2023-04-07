using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

using Refit;
using System.Net;
using System.Net.Http.Headers;
using Ecommerce.Client.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Ecommerce.Client.DelegatingHandlers;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IConfiguration _configuration;

    public RefreshTokenHandler(IHttpClientFactory httpFactory, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        => (_httpClientFactory, _contextAccessor, _configuration) = (httpFactory, contextAccessor, configuration);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is not HttpStatusCode.Unauthorized)
            return response;

        var authResponse = await RefreshTokenRequestWrapper(request);

        RemoveExpireTokenCookies();

        SetRefreshedTokenCookies(authResponse);

        SetAuthorizationHeaderForNextRequest(request, authResponse);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<AuthenticateResponse> RefreshTokenRequestWrapper(HttpRequestMessage request)
    {
        var client = RestService.For<IEcommerceApi>(GetHttpClient());

        try
        {
            return await client.RefreshToken(GetRefreshTokenRequestFromHeaders(request));
        }
        catch
        {
            throw await ApiException.Create(request, HttpMethod.Post, new HttpResponseMessage(HttpStatusCode.Unauthorized), new());
        }
    }

    HttpClient GetHttpClient()
    {
        string baseUrl = _configuration.GetSection("ApiUrl").Value!;

        var client = _httpClientFactory.CreateClient();

        client.BaseAddress = new Uri(baseUrl);

        return client;
    }

    static RefreshTokenRequest GetRefreshTokenRequestFromHeaders(HttpRequestMessage req)
    {
        string? accessToken = req.Headers.Authorization!.Parameter;
        string refreshTokenEncoded = req.Headers.FirstOrDefault(h => h.Key == "refresh-token").Value.First();
        string refreshToken = DecodeToken(refreshTokenEncoded);

        return new RefreshTokenRequest(accessToken!, refreshToken);
    }

    static string DecodeToken(string refreshTokenEncoded)
        => WebUtility.UrlDecode(refreshTokenEncoded).Replace(" ", "+");

    void RemoveExpireTokenCookies()
    {
        _contextAccessor.HttpContext!.Response.Cookies.Delete("refresh-token");
        _contextAccessor.HttpContext!.Response.Cookies.Delete("access-token");
    }

    void SetRefreshedTokenCookies(AuthenticateResponse authResponse)
    {
        _contextAccessor.HttpContext!.Response.Cookies.Append("access-token", authResponse!.AccessToken);
        _contextAccessor.HttpContext!.Response.Cookies.Append("refresh-token", authResponse!.RefreshToken);
    }

    private static void SetAuthorizationHeaderForNextRequest(HttpRequestMessage request, AuthenticateResponse authResponse)
    => request.Headers.Authorization = new AuthenticationHeaderValue(
                                                    JwtBearerDefaults.AuthenticationScheme,
                                                    authResponse!.AccessToken);
}