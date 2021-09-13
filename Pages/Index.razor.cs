using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using battleship.Models;

namespace battleship.Pages
{
    public partial class Index
    {
        public Player RedPlayer { get; set; }

        public Player BluePlayer { get; set; }

        public Dictionary<int, int> Ships { get; set; }

        public int FieldWidth { get; set; }

        public int FieldHeight { get; set; }

        protected override void OnInitialized()
        {
            Ships = new Dictionary<int, int>{
                {5,1},
                {4,1},
                {3,2},
                {2,1}
            };

            Console.WriteLine("Initializing fields");

            FieldHeight = 10;
            FieldWidth = 10;

            int maxPoints = Ships.Sum(x => x.Value * x.Key);
            Console.WriteLine($"Points assigned to players: {maxPoints}");

            RedPlayer = new Player
            {
                GameField = new GameField(FieldWidth, FieldHeight),
                PlayerMoves = new List<Move>(),
                PointsLeft = maxPoints
            };

            BluePlayer = new Player
            {
                GameField = new GameField(FieldWidth, FieldHeight),
                PlayerMoves = new List<Move>(),
                PointsLeft = maxPoints
            };
        }

        public void PlaceShips()
        {
            Console.WriteLine("Placing ships");

            _gameService.ClearGameField(RedPlayer.GameField);

            _gameService.ClearGameField(BluePlayer.GameField);

            _gameService.PlaceShips(Ships, RedPlayer.GameField);

            _gameService.PlaceShips(Ships, BluePlayer.GameField);
        }

        public async void StartGame()
        {
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

            while (shootingPlayer.PlayerMoves.Any(m => m.CoordinateX == x && m.CoordinateY == y))
            {
                x = rand.Next(0, FieldWidth);
                y = rand.Next(0, FieldHeight);
            }

            var redResult = _gameService.Fire(shootingPlayer, targetPlayer, x, y);

            if (redResult == CellState.Hit)
            {
                targetPlayer.PointsLeft--;
                Console.WriteLine($"{playerName} Hit! Enemy score: {targetPlayer.PointsLeft}");
            }

            await Task.Delay(100);
            StateHasChanged();
        }
    }
}