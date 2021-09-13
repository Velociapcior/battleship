namespace battleship.Models
{
    public class Move
    {
        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        public Cell Target { get; set; }
    }
}