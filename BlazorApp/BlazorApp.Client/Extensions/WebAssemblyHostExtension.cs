using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

namespace BlazorApp.Client.Extensions
{
    public static class WebAssemblyHostExtension
    {
        public static CultureInfo[] SupportedCultures { get; } =
        [
            new CultureInfo("en-US"),
            new CultureInfo("es-ES"),
        ];

        public static async Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var localStorage = host.Services.GetRequiredService<ILocalStorageService>();

            var storedCulture = await localStorage.GetItemAsync<string>("CurrentCulture");
            if (storedCulture == null)
            {
                await localStorage.SetItemAsync("CurrentCulture", SupportedCultures[0]);
            }

            var culture = CultureInfo.GetCultureInfo(storedCulture ?? CultureInfo.CurrentCulture.Name);
            if (!SupportedCultures.Select(x => x.Name).Contains(culture.Name))
            {
                culture = SupportedCultures[0];
                await localStorage.SetItemAsync(key: "CurrentCulture", SupportedCultures[0]);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}