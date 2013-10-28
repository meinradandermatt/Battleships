using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships
{
    public class ShipValidator
    {
        public static void ValidateShips(IEnumerable<IEnumerable<Position>> shipCoordinates)
        {
            ValidateShips(ShipParser.Parse(shipCoordinates));
        }

        public static void ValidateShips(IEnumerable<Ship> ships)
        {
            ValidateOverlappingCoordinates(ships);
            ValidateNumberOfShips(ships);
            ValidateBoardBoundaries(ships);
            ValidateShipCoordinates(ships);
            ValidateLengthOfShips(ships);
        }

        private static void ValidateLengthOfShips(IEnumerable<Ship> ships)
        {
            var groupedByLength = ships.GroupBy(s => s.Positions.Count()).OrderByDescending(g => g.Key);
            var shipLengths = string.Join(", ", groupedByLength.Select(g => string.Format("{0}x {1}", g.Count(), g.Key)));
            const string requiredShips = "1x 5, 2x 4, 3x 3, 4x 2";
            if (shipLengths != requiredShips)
            {
                throw new ArgumentException(string.Format("Expected the following ships: {0}. But Received {1}",
                    requiredShips,
                    shipLengths));
            }
        }

        private static void ValidateShipCoordinates(IEnumerable<Ship> ships)
        {
            foreach (Ship ship in ships)
            {
                int maxX = ship.Positions.Max(p => p.X);
                int minX = ship.Positions.Min(p => p.X);
                int maxY = ship.Positions.Max(p => p.Y);
                int minY = ship.Positions.Min(p => p.Y);

                if (maxX == minX && (maxY - minY) == ship.Positions.Count() - 1)
                {
                    continue;
                }

                if (maxY == minY && (maxX - minX) == ship.Positions.Count() - 1)
                {
                    continue;
                }

                throw new ArgumentException(string.Format("ship has an invalid set of coordinates: {0}",
                    string.Join(",", ship.Positions.Select(p => string.Format("[{0},{1}]", p.X, p.Y)))));
            }
        }

        private static void ValidateBoardBoundaries(IEnumerable<Ship> ships)
        {
            if (ships.Any(s => s.Positions.Any(p => p.X > 9 || p.X < 0 || p.Y > 9 || p.Y < 0)))
            {
                throw new ArgumentException("not all ships are within the allowed boundaries (0-9)");
            }
        }

        private static void ValidateNumberOfShips(IEnumerable<Ship> ships)
        {
            int numberOfShips = ships.Count();
            if (numberOfShips != 10)
            {
                throw new ArgumentException(string.Format("provided {0} instead of 10 ships", numberOfShips));
            }
        }

        private static void ValidateOverlappingCoordinates(IEnumerable<Ship> ships)
        {
            var orderedByPosition = ships.SelectMany(s => s.Positions).GroupBy(p => p);
            IGrouping<Position, Position> overlapping = orderedByPosition.FirstOrDefault(p => p.Count() > 1);
            if (overlapping != null)
            {
                var overlappingPosition = overlapping.Key;
                throw new ArgumentException(string.Format("position [{0},{1}] is used multiple times",
                    overlappingPosition.X,
                    overlappingPosition.Y));
            }
        }
    }
}
