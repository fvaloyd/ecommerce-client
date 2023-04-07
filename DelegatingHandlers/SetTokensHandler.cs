using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Ecommerce.Client.DelegatingHandlers;

public class SetTokensHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public SetTokensHandler(IHttpContextAccessor contextAccessor)
        => _contextAccessor = contextAccessor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) = GetTokensFromCurrentRequestCookies();

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return base.SendAsync(request, cancellationToken);

        SetTokensForTheCurrentRequest(request, accessToken, refreshToken);

        return base.SendAsync(request, cancellationToken);
    }

    (string? accessToken, string? refreshToken) GetTokensFromCurrentRequestCookies()
    {
        var accessToken = _contextAccessor.HttpContext!.Request.Cookies["access-token"];
        var refreshToken = _contextAccessor.HttpContext.Request.Cookies["refresh-token"];

        return (accessToken, refreshToken);
    }

    static void SetTokensForTheCurrentRequest(HttpRequestMessage request, string accessToken, string refreshToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        request.Headers.Add("refresh-token", new[] { refreshToken });
    }
}