using BlazorComponentUtilities;

namespace BlazorApp.Client.Components
{
    public sealed partial class GameBoard
    {
        private string? NextWordClasses => new CssBuilder()
            .AddClass("flex", ShowNextWord)
            .AddClass("hidden", !ShowNextWord)
            .Build();

        private string? KeyboardContainerClasses => new CssBuilder()
            .AddClass("flex", ShowKeyboard)
            .AddClass("hidden", !ShowKeyboard)
            .Build();

        private bool ShowNextWord => GameManagerService.GameState == GameState.Win ||
                GameManagerService.GameState == GameState.GameOver;

        private bool ShowKeyboard => GameManagerService.GameState == GameState.NotStarted ||
                GameManagerService.GameState == GameState.Playing;

        protected override async Task OnInitializedAsync()
        {
            MessageBusService.OnBoardGameRefresh += NotifyChange;
            await GameManagerService.StartGame();
        }

        private void NotifyChange()
        {
            InvokeAsync(StateHasChanged);
        }

        private string GetNextWordMessage()
        {
            if (GameManagerService.GameState == GameState.Win)
                return LocalizationService["GameboardWin"];
            else
                return LocalizationService["GameboardLose"];
        }

        private void ShowStats()
        {
            MessageBusService.TriggerStats();
        }

        public void Dispose()
        {
            MessageBusService.OnBoardGameRefresh -= NotifyChange;
        }
    }
}