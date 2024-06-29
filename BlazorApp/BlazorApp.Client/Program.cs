using BlazorApp.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();
builder.Services.AddScoped<LocalizationService>();

builder.Services.AddScoped<GameManagerService>();
builder.Services.AddScoped<ToastNotificationService>();
builder.Services.AddScoped<BrowserLocalStorageService>();
builder.Services.AddBlazoredLocalStorage();

var host = builder.Build();

await host.SetDefaultCulture();

await host.RunAsync();