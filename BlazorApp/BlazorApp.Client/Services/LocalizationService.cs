using Microsoft.Extensions.Localization;

namespace BlazorApp.Client.Services
{
    public class LocalizationService
    {
        public LocalizedString this[string name] => GetLocalizedString(name);

        private Language _currentLanguage;
        public Language CurrentLanguage { get => _currentLanguage; }

        private readonly IStringLocalizer<Localization> _localizer;
        private readonly ILocalStorageService _localStorage;

        public LocalizationService(IStringLocalizer<Localization> localizer, ILocalStorageService localStorage)
        {
            _localizer = localizer;
            _localStorage = localStorage;
        }

        public async Task GetCurrentCulture()
        {
            var currentCulture = await _localStorage.GetItemAsync<string>("CurrentCulture");

            if (currentCulture?.StartsWith("es") == true)
                _currentLanguage = Language.Spanish;
            else
                _currentLanguage = Language.English;
        }

        public async Task SwitchLanguage()
        {
            if (CurrentLanguage == Language.English)
            {
                await _localStorage.SetItemAsync("CurrentCulture", "es-ES");
            }
            else
            {
                await _localStorage.SetItemAsync("CurrentCulture", "en-US");
            }
        }

        private LocalizedString GetLocalizedString(string name)
        {
            return _localizer[name];
        }
    }
}