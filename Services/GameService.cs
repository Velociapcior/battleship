using System;
using System.Collections.Generic;
using battleship.Models;
using battleship.Services.Interfaces;

namespace battleship.Services
{
    public class GameService : IGameService
    {
        public void ClearGameField(GameField gameField)
        {
            for (int i = 0; i < gameField.FieldWidth; i++)
            {
                for (int j = 0; j < gameField.FieldHeight; j++)
                {
                    gameField.SetCell(i, j, CellState.Empty, ShipType.None);
                }
            }
        }

        public void PlaceShips(IDictionary<ShipType, int> ships, GameField gameField)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            foreach (var ship in ships)
            {
                for (int i = 0; i < ship.Value; i++)
                {
                    bool placedShip = false;

                    while (!placedShip)
                    {
                        int x = rand.Next(0, gameField.FieldWidth);
                        int y = rand.Next(0, gameField.FieldHeight);

                        Direction direction = (Direction)rand.Next(0, 2);

                        if (!gameField.CheckIfShipCanBePlaced(x, y, (int) ship.Key, direction))
                        {
                            continue;
                        }

                        switch (direction)
                        {
                            case Direction.Horizontal:
                                for (int j = x; j < x + (int) ship.Key; j++)
                                {
                                    gameField.SetCell(j, y, CellState.Ship, ship.Key);
                                }
                                break;

                            case Direction.Vertical:
                                for (int j = y; j < y + (int) ship.Key; j++)
                                {
                                    gameField.SetCell(x, j, CellState.Ship, ship.Key);
                                }
                                break;
                        }

                        placedShip = true;
                    }
                }
            }
        }

        public CellState Fire(Player shootingPlayer, Player targetPlayer, int x, int y)
        {
            var attackedCell = targetPlayer.GameField.GetCell(x, y);

            if (attackedCell.CellState == CellState.Empty)
            {
                targetPlayer.GameField.SetCell(x, y, CellState.Miss);
                shootingPlayer.PlayerMoves.Add(new Move { CoordinateX = x, CoordinateY = y, Target = attackedCell});
                return CellState.Miss;
            }
            else
            {
                targetPlayer.GameField.SetCell(x, y, CellState.Hit);
                shootingPlayer.PlayerMoves.Add(new Move { CoordinateX = x, CoordinateY = y, Target = attackedCell });
                return CellState.Hit;
            }
        }
    }
}