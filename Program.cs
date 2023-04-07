using Ecommerce.Client.Extensions;
using Ecommerce.Client.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(config => {
    config.Filters.Add(typeof(ApiExceptionFilter));
});

builder.Services.AddRazorPages();

builder.Services.AddCookieAuthentication()
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