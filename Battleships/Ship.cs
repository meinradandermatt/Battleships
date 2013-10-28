using System.Collections.Generic;

namespace Battleships
{
    public class Ship
    {
        public Ship(IEnumerable<Position> positions)
        {
            Positions = positions;
            this.Sunk = false;
        }

        public IEnumerable<Position> Positions { get; private set; }

        public bool Sunk { get; set; }
    }
}
