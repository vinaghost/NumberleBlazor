using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components
{
    public sealed partial class BoardLine
    {
        [Parameter, EditorRequired]
        public int RowIndex { get; set; }

        [Parameter, EditorRequired]
        public RenderFragment ChildContent { get; set; } = default!;

        private Animation _animationRef = default!;

        protected override void OnInitialized()
        {
            MessageBusService.OnBoardLineWrongSolution += TriggerAnimation;
        }

        public void Dispose()
        {
            MessageBusService.OnBoardLineWrongSolution -= TriggerAnimation;
        }

        private void TriggerAnimation(int currentRow)
        {
            if (currentRow == RowIndex)
                _animationRef.TriggerAnimation();
        }
    }
}