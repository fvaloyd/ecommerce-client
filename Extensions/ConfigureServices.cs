using Ecommerce.Client.Services;
using Ecommerce.Client.DelegatingHandlers;

using Refit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ecommerce.Client.Filters;

namespace Ecommerce.Client.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddWebConfiguration(this IServiceCollection services)
    {
        services.AddMvc(config => {
            config.Filters.Add(typeof(ApiExceptionFilter));
        });

        services.AddRazorPages();

        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
        {
            config.Cookie.Name = "CookieAuth";
            config.Cookie.HttpOnly = true;
            config.Cookie.SameSite = SameSiteMode.Strict;
            config.ExpireTimeSpan = TimeSpan.FromHours(3);

            config.AccessDeniedPath = "/Auth/AccessDenied";
            config.LoginPath = "/Auth/Login";
            config.LogoutPath = "/Auth/Logout";
        });

        return services;
    }

    public static IServiceCollection AddRefitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        string ApiUrl = configuration.GetSection("ApiUrl").Value!;

        services.AddTransient<RefreshTokenHandler>();
        services.AddTransient<SetTokensHandler>();

        services.AddRefitClient<IEcommerceApi>()
                        .ConfigureHttpClient(c => c.BaseAddress = new Uri(ApiUrl))
                        .AddHttpMessageHandler<RefreshTokenHandler>()
                        .AddHttpMessageHandler<SetTokensHandler>();

        return services;
    }
}