using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApp.Client.Pages
{
    public partial class Numberle
    {
        private readonly IReadOnlyList<string> _validKeys = new List<string>()
        {
            "ENTER", "BACKSPACE",
            "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
            "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ñ",
            "Z", "X", "C", "V", "B", "N", "M"
        };

        private readonly ElementReference _mainDiv;
        private readonly GameBoard? _gameBoard;
        private bool _showStats;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await _mainDiv.FocusAsync();
            }
        }

        public void ShowStats()
        {
            _showStats = true;
            StateHasChanged();
        }

        private void CloseStats()
        {
            _showStats = false;
            StateHasChanged();
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

                _gameBoard?.NotifyChange();
            }
        }
    }
}