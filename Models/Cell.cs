namespace battleship.Models
{
    // podstawowy element pola gry, pojedyncza komórka
    public class Cell
    {
        public CellState CellState { get; set; }

        public Cell()
        {
            CellState = CellState.Empty;
        }
    }
}