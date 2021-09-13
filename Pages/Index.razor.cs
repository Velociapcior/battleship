using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using battleship.Models;

namespace battleship.Pages
{
    public partial class Index
    {
        private bool _areShipsPlaced = false;

        public Player RedPlayer { get; set; }

        public Player BluePlayer { get; set; }

        public Dictionary<ShipType, int> Ships { get; set; }

        public int FieldWidth { get; set; }

        public int MaxPoints { get; private set; }

        public int FieldHeight { get; set; }

        protected override void OnInitialized()
        {
            Ships = new Dictionary<ShipType, int>{
                {ShipType.Carrier,1},
                {ShipType.Battleship,1},
                {ShipType.Destroyer,2},
                {ShipType.PatrolBoat,1}
            };

            FieldHeight = 10;
            FieldWidth = 10;

            MaxPoints = Ships.Sum(x => x.Value * (int)x.Key);

            RedPlayer = new Player
            {
                GameField = new GameField(FieldWidth, FieldHeight),
                PlayerMoves = new List<Move>(),
                PointsLeft = MaxPoints
            };

            BluePlayer = new Player
            {
                GameField = new GameField(FieldWidth, FieldHeight),
                PlayerMoves = new List<Move>(),
                PointsLeft = MaxPoints
            };
        }

        public void PlaceShips()
        {
            RedPlayer.PointsLeft = BluePlayer.PointsLeft = MaxPoints;

            RedPlayer.PlayerMoves = new List<Move>();

            BluePlayer.PlayerMoves = new List<Move>();

            _gameService.ClearGameField(RedPlayer.GameField);

            _gameService.ClearGameField(BluePlayer.GameField);

            _gameService.PlaceShips(Ships, RedPlayer.GameField);

            _gameService.PlaceShips(Ships, BluePlayer.GameField);

            _areShipsPlaced = true;
        }

        public async void StartGame()
        {
            if(!_areShipsPlaced){
                PlaceShips();
            }

            while (RedPlayer.PointsLeft > 0 || BluePlayer.PointsLeft > 0)
            {
                //Red player move
                await TakeShot("red");
                if (BluePlayer.PointsLeft == 0)
                {
                    break;
                }

                //Blue player move
                await TakeShot("blue");
                if (RedPlayer.PointsLeft == 0)
                {
                    break;
                }
            }

            _areShipsPlaced = false;
        }

        public async Task TakeShot(string playerName)
        {
            Player shootingPlayer = new Player();
            Player targetPlayer = new Player();

            switch (playerName)
            {
                case "red":
                    shootingPlayer = RedPlayer;
                    targetPlayer = BluePlayer;
                    break;
                case "blue":
                    shootingPlayer = BluePlayer;
                    targetPlayer = RedPlayer;
                    break;
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());

            int x = rand.Next(0, FieldWidth);
            int y = rand.Next(0, FieldHeight);

            var lastSuccessfullMove = shootingPlayer.PlayerMoves.LastOrDefault(x => x.Target.CellState == CellState.Hit);

            if (lastSuccessfullMove != null)
            {
                var surroundingMoves = new List<Move>{
                    new Move {CoordinateX = lastSuccessfullMove.CoordinateX - 1, CoordinateY = lastSuccessfullMove.CoordinateY},
                    new Move {CoordinateX = lastSuccessfullMove.CoordinateX + 1, CoordinateY = lastSuccessfullMove.CoordinateY},
                    new Move {CoordinateX = lastSuccessfullMove.CoordinateX, CoordinateY = lastSuccessfullMove.CoordinateY - 1},
                    new Move {CoordinateX = lastSuccessfullMove.CoordinateX , CoordinateY = lastSuccessfullMove.CoordinateY + 1},
                }.Where(x => x.CoordinateX >= 0 && x.CoordinateX < FieldWidth &&
                             x.CoordinateY >= 0 && x.CoordinateY < FieldHeight).ToList();

                var nextMove = surroundingMoves
                .Where(sm => !shootingPlayer.PlayerMoves
                .Any(pm => pm.CoordinateX == sm.CoordinateX && pm.CoordinateY == sm.CoordinateY)).ToList().FirstOrDefault();

                if (nextMove != null)
                {
                    Console.WriteLine($"Found next move {nextMove.CoordinateX}, {nextMove.CoordinateY}");
                    x = nextMove.CoordinateX;
                    y = nextMove.CoordinateY;
                }
            }

            while (shootingPlayer.PlayerMoves.Any(m => m.CoordinateX == x && m.CoordinateY == y))
            {
                x = rand.Next(0, FieldWidth);
                y = rand.Next(0, FieldHeight);
            }

            var redResult = _gameService.Fire(shootingPlayer, targetPlayer, x, y);

            if (redResult == CellState.Hit)
            {
                targetPlayer.PointsLeft--;
            }

            await Task.Delay(100);
            StateHasChanged();
        }
    }
}