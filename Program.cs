using Ecommerce.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebConfiguration()
                .AddCookieAuthentication()
                .AddAuthorization()
                .AddRefitConfiguration(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();