namespace BlazorApp.Client.Components
{
    public partial class HeaderBar
    {
        private string GetCurrentLanguageFlagPath()
        {
            if (LocalizationService.CurrentLanguage == Language.English)
            {
                return "images/english.svg";
            }
            else
            {
                return "images/spanish.svg";
            }
        }

        private async Task ChangeLanguage()
        {
            await LocalizationService.SwitchLanguage();

            NavManager.NavigateTo(NavManager.Uri, forceLoad: true);
        }
    }
}