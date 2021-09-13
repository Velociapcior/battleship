using System;
using System.Reflection.Emit;
namespace battleship.Models
{
    public class GameField
    {
        private readonly Cell[,] _cells;

        public int FieldHeight { get => _fieldHeight; }

        public int FieldWidth { get => _fieldWidth; }

        public GameField(int fieldWidth, int fieldHeight)
        {
            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;


            _cells = new Cell[fieldWidth, fieldHeight];

            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    _cells[i, j] = new Cell { CellState = CellState.Empty };
                }
            }
        }

        private readonly int _fieldWidth;

        private readonly int _fieldHeight;

        public void SetCell(int x, int y, CellState cellState)
        {
            if (x >= _fieldWidth || y >= _fieldHeight)
            {
                throw new IndexOutOfRangeException($"Coordinates are out of range: x={x}, field width: {_fieldWidth} or y={y}, field height: {_fieldHeight}");
            }

            _cells[x, y].CellState = cellState;
        }

        public void SetCell(int x, int y, CellState cellState, ShipType shipType)
        {
            if (x >= _fieldWidth || y >= _fieldHeight)
            {
                throw new IndexOutOfRangeException($"Coordinates are out of range: x={x}, field width: {_fieldWidth} or y={y}, field height: {_fieldHeight}");
            }

            _cells[x, y].CellState = cellState;
            _cells[x, y].ShipType = shipType;
        }

        public Cell GetCell(int x, int y)
        {
            if (x >= _fieldWidth || y >= _fieldHeight)
            {
                throw new IndexOutOfRangeException($"Coordinates are out of range: x={x}, field width: {_fieldWidth} or y={y}, field height: {_fieldHeight}");
            }

            return _cells[x, y];
        }

        public bool CheckIfShipCanBePlaced(int x, int y, int shipLength, Direction shipDirection)
        {
            int shipXEnd = shipDirection == Direction.Horizontal ? x + shipLength - 1 : 1;
            int shipYEnd = shipDirection == Direction.Vertical ? y + shipLength - 1 : 1;

            if (shipXEnd >= _fieldWidth || shipYEnd >= _fieldHeight)
            {
                return false;
            }

            switch (shipDirection)
            {
                case Direction.Horizontal:
                    return CheckHorizontal(x, y, shipXEnd);

                case Direction.Vertical:
                    return CheckVertical(x, y, shipYEnd);
                default:
                    return false;
            }
        }

        private bool CheckVertical(int x, int y, int shipYEnd)
        {
            for (int i = y; i < shipYEnd; i++)
            {
                //ship beggining
                if (i > 0 && i == y)
                {
                    if (!IsCellEmpty(x, i - 1))
                    {
                        return false;
                    }
                }

                //ship end
                if (i < FieldHeight - 1 && i == shipYEnd - 1)
                {
                    if (!IsCellEmpty(x, i + 1))
                    {
                        return false;
                    }
                }

                //ship side
                if (x > 0)
                {
                    if (!IsCellEmpty(x - 1, i))
                    {
                        return false;
                    }
                }

                if (x < FieldWidth - 1)
                {
                    if (!IsCellEmpty(x + 1, i))
                    {
                        return false;
                    }
                }


                if (!IsCellEmpty(x, i))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckHorizontal(int x, int y, int shipXEnd)
        {
            for (int i = x; i < shipXEnd; i++)
            {
                // ship beggining
                if (i > 0 && i == x)
                {
                    IsCellEmpty(i - 1, y);

                    if (!IsCellEmpty(i - 1, y))
                    {
                        return false;
                    }
                }

                // ship end
                if (i < FieldWidth - 1 && i == shipXEnd - 1)
                {
                    if (!IsCellEmpty(i + 1, y))
                    {
                        return false;
                    }
                }

                //ship side
                if (y > 0)
                {
                    if (!IsCellEmpty(i, y - 1))
                    {
                        return false;
                    }
                }

                if (y < FieldHeight - 1)
                {
                    if (!IsCellEmpty(i, y + 1))
                    {
                        return false;
                    }
                }

                if (!IsCellEmpty(i, y))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsCellEmpty(int x, int y) => GetCell(x, y).CellState == CellState.Empty;
    }
}