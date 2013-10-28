using System;
using System.Linq;

namespace Battleships
{
    public class BattleshipGame
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public Player CurrentPlayer { get; set; }
        public bool Finished { get; set; }

        public BattleshipGame(Player player1, Player player2)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.CurrentPlayer = player1;
        }

        public PublicFieldStates MakeMove(int x, int y)
        {
            if (this.Finished)
            {
                throw new InvalidOperationException("This game has ended");
            }

            var enemy = this.CurrentPlayer == this.Player1 ? this.Player2 : this.Player1;

            Ship ship = enemy.Ships.SingleOrDefault(s => s.Positions.Any(p => p.Equals(new Position(x, y))));
            var result = PublicFieldStates.Empty;
            if (ship != null)
            {
                result = PublicFieldStates.Hit;
            }
            enemy.PublicBoard[y, x] = result;

            if (result == PublicFieldStates.Hit)
            {
                result = CheckIfShipSunk(ship, enemy);
            }

            if (enemy.Ships.All(s => s.Positions.All(p => enemy.PublicBoard[p.Y, p.X] == PublicFieldStates.Sunk)))
            {
                this.Finished = true;
            }
            else
            {
                this.CurrentPlayer = enemy;
            }

            return result;
        }

        private static PublicFieldStates CheckIfShipSunk(Ship ship, Player enemy)
        {
            bool wholeShipHit =
                ship.Positions.All(position => enemy.PublicBoard[position.Y, position.X] == PublicFieldStates.Hit);

            if (wholeShipHit)
            {
                foreach (Position position in ship.Positions)
                {
                    enemy.PublicBoard[position.Y, position.X] = PublicFieldStates.Sunk;
                }
                ship.Sunk = true;

                return PublicFieldStates.Sunk;
            }

            return PublicFieldStates.Hit;
        }
    }
}
