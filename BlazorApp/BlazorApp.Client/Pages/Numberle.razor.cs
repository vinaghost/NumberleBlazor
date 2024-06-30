using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApp.Client.Pages
{
    public sealed partial class Numberle
    {
        private readonly IReadOnlyList<string> _validKeys =
        [
            "ENTER", "BACKSPACE",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        ];

        private ElementReference _mainDiv;
        private bool _showStats;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _mainDiv.FocusAsync();
            }
        }

        protected override void OnInitialized()
        {
            MessageBusService.OnStatsTrigger += ShowStats;
        }

        public void Dispose()
        {
            MessageBusService.OnStatsTrigger -= ShowStats;
        }

        public void ShowStats()
        {
            _showStats = true;
            InvokeAsync(StateHasChanged);
        }

        private async Task CloseStats()
        {
            _showStats = false;
            await InvokeAsync(StateHasChanged);
        }

        private async Task KeyDown(KeyboardEventArgs e)
        {
            string key = e.Key.ToUpper();
            if (GameManagerService.GameState == GameState.Playing && _validKeys.Contains(key))
            {
                if (key == "ENTER")
                {
                    await GameManagerService.CheckCurrentLineSolution();
                }
                else if (key == "BACKSPACE")
                {
                    GameManagerService.RemoveLastValue();
                }
                else
                {
                    GameManagerService.EnterNextValue(Convert.ToChar(key));
                }

                MessageBusService.RefreshBoardGame();
            }
        }
    }
}