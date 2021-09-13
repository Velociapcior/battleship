using System.Collections.Generic;
using battleship.Models;

namespace battleship.Services.Interfaces
{
    public interface IGameService
    {
        CellState Fire(Player shootingPlayer, Player targetPlayer, int x, int y);

        void ClearGameField(GameField gameField);

        void PlaceShips(IDictionary<int, int> ships, GameField gameField);
    }
}