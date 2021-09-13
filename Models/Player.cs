using System.Collections.Generic;

namespace battleship.Models
{
    public class Player
    {
        public GameField GameField { get; set; }

        public List<Move> PlayerMoves { get; set; }

        public int PointsLeft { get; set; }
    }
}