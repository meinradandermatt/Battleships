using System.Collections.Generic;

namespace Battleships
{
    public class GameFactory
    {
        public static BattleshipGame Create(
            string player1Name,
            IEnumerable<IEnumerable<Position>> player1Ships,
            string player2Name,
            IEnumerable<IEnumerable<Position>> player2Ships)
        {
            var ships1 = ShipParser.Parse(player1Ships);
            var ships2 = ShipParser.Parse(player2Ships);

            ShipValidator.ValidateShips(ships1);
            ShipValidator.ValidateShips(ships2);

            return new BattleshipGame(
                new Player
                {
                    Name = player1Name,
                    Ships = ships1
                },
                new Player
                {
                    Name = player2Name,
                    Ships = ships2
                });
        }
    }
}