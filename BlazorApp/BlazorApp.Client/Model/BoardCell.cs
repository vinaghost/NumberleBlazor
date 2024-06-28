using BlazorApp.Client.Model.Enums;

namespace BlazorApp.Client.Model
{
    public class BoardCell
    {
        public char? Value { get; set; }
        public BoardCellState State { get; set; } = BoardCellState.Empty;
    }
}
