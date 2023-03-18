using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Ecommerce.Client.BackendClient.MessageHandlers;

public class SetTokensHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor contextAccessor;

    public SetTokensHandler(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = contextAccessor.HttpContext!.Request.Cookies["access-token"];
        var refreshToken = contextAccessor.HttpContext.Request.Cookies["refresh-token"];

        if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken))
        {
            return base.SendAsync(request, cancellationToken);
        }
        
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
        request.Headers.Add("refresh-token", new[] {refreshToken});

        return base.SendAsync(request, cancellationToken);
    }
}