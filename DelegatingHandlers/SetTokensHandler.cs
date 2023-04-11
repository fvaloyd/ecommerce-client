using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Ecommerce.Client.DelegatingHandlers;

public class SetTokensHandler : DelegatingHandler
{
    readonly IHttpContextAccessor _contextAccessor;

    public SetTokensHandler(IHttpContextAccessor contextAccessor)
        => _contextAccessor = contextAccessor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) = GetTokensFromCurrentClientRequestCookies();

        if (IsNullOrEmptyTokens(accessToken, refreshToken))
            return base.SendAsync(request, cancellationToken);

        SetTokensForTheNextRequest(request, accessToken!, refreshToken!);

        return base.SendAsync(request, cancellationToken);
    }

    (string? accessToken, string? refreshToken) GetTokensFromCurrentClientRequestCookies()
    {
        var accessToken = _contextAccessor.HttpContext!.Request.Cookies["access-token"];
        var refreshToken = _contextAccessor.HttpContext.Request.Cookies["refresh-token"];

        return (accessToken, refreshToken);
    }

    static bool IsNullOrEmptyTokens(string? accessToken, string? refreshToken)
        => string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken);

    static void SetTokensForTheNextRequest(HttpRequestMessage request, string accessToken, string refreshToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        request.Headers.Add("refresh-token", new[] { refreshToken });
    }
}