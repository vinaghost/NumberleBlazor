using BlazorComponentUtilities;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components
{
    public sealed partial class Key
    {
        [Parameter]
        public char KeyValue { get; set; }

        [Parameter]
        public KeyType KeyType { get; set; }

        private string KeyClasses => new CssBuilder()
            .AddClass("h-[52px] cursor-pointer")
            .AddClass("flex justify-center items-center grow")
            .AddClass("text-center font-bold text-[15px]")
            .AddClass("rounded-md border-transparent pb-0.5 px-1 mx-0.5")
            .AddClass("transition-keyactive duration-150")
            .AddClass("hover:bg-keyhover active:bg-keyactive active:border-[3px] active:border-keyactiveborder")
            .AddClass(GetKeyStateClass())
            .AddClass("w-[44px]", KeyType == KeyType.Number)
            .AddClass("w-[66px]", KeyType != KeyType.Number).Build();

        private string GetKeyStateClass()
        {
            var keyStatus = GameManagerService.UsedKeys.FirstOrDefault(x => x.Key == KeyValue);

            if (keyStatus.Equals(default(KeyValuePair<char, KeyState>)))
            {
                return "bg-middlegray";
            }

            return keyStatus.Value switch
            {
                KeyState.Correct => "bg-green border-green text-white hover:bg-greenhover",
                KeyState.IncorrectPosition => "bg-yellow border-yellow text-white hover:bg-yellowhover",
                KeyState.Wrong => "bg-darkgray border-darkgray text-white hover:bg-darkgrayhover",
                _ => "bg-middlegray"
            };
        }

        public void NotifyChange(char key)
        {
            if (key == KeyValue)
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private async Task KeyClicked()
        {
            if (KeyType == KeyType.Number)
            {
                GameManagerService.EnterNextValue(KeyValue);
            }
            else if (KeyType == KeyType.Send)
            {
                await GameManagerService.CheckCurrentLineSolution();
            }
            else if (KeyType == KeyType.Remove)
            {
                GameManagerService.RemoveLastValue();
            }

            MessageBusService.RefreshBoardGame();
        }

        private string GetKeyText()
        {
            if (KeyType == KeyType.Number)
            {
                return KeyValue.ToString();
            }
            else if (KeyType == KeyType.Send)
            {
                return LocalizationService["KeyboardSend"];
            }
            else if (KeyType == KeyType.Remove)
            {
                return LocalizationService["KeyboardDel"];
            }
            else
            {
                return "";
            }
        }

        protected override void OnInitialized()
        {
            MessageBusService.OnKeyRefresh += NotifyChange;
        }

        public void Dispose()
        {
            MessageBusService.OnKeyRefresh -= NotifyChange;
        }
    }
}