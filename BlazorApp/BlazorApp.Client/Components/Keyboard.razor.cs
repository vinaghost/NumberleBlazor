using Microsoft.AspNetCore.Components;

namespace BlazorApp.Client.Components
{
    public partial class Keyboard
    {
        [CascadingParameter]
        public GameBoard? AncestorComponent { get; set; }
    }
}