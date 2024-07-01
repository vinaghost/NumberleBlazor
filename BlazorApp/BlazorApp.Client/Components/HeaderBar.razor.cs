using System.Globalization;

namespace BlazorApp.Client.Components
{
    public partial class HeaderBar
    {
        private CultureInfo? selectedCulture;

        private static CultureInfo[] SupportedCultures { get; } =
        [
            new CultureInfo("en-US"),
            new CultureInfo("es-ES"),
        ];

        protected override void OnInitialized()
        {
            selectedCulture = CultureInfo.CurrentCulture;
        }

        private async Task ApplySelectedCultureAsync()
        {
            if (CultureInfo.CurrentCulture != selectedCulture)
            {
                await LocalStorage.SetItemAsync("CurrentCulture", selectedCulture!.Name);

                NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
            }
        }
    }
}