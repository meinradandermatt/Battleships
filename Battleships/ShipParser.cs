using System.Collections.Generic;
using System.Linq;

namespace Battleships
{
    public class ShipParser
    {
        public static  IEnumerable<Ship> Parse(IEnumerable<IEnumerable<Position>> shipPositions)
        {
            return shipPositions.Where(p => p.Any()).Select(shipCoordinates => new Ship(shipCoordinates));
        }
    }
}
