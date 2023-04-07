using Refit;
using Ecommerce.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ecommerce.Client.BackendClient.MessageHandlers;

namespace Ecommerce.Client.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
        {
            config.Cookie.Name = "CookieAuth";
            config.Cookie.HttpOnly = true;
            config.Cookie.SameSite = SameSiteMode.Strict;
            config.ExpireTimeSpan = TimeSpan.FromDays(1);

            config.AccessDeniedPath = "/Auth/AccessDenied";
            config.LoginPath = "/Auth/Login";
            config.LogoutPath = "/Auth/Logout";
        });

        return services;
    }

    public static IServiceCollection AddRefitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        string ApiUrl = configuration.GetSection("ApiUrl").Value!;

        services.AddHttpContextAccessor();

        services.AddTransient<RefreshTokenHandler>();
        services.AddTransient<SetTokensHandler>();

        services.AddRefitClient<IEcommerceApi>()
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri(ApiUrl))
                        .AddHttpMessageHandler<RefreshTokenHandler>()
                        .AddHttpMessageHandler<SetTokensHandler>();

        return services;
    }
}
