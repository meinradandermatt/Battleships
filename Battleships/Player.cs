using System.Collections.Generic;

namespace Battleships
{
    public class Player
    {
        public Player()
        {
            this.PublicBoard = new PublicFieldStates[10,10];
            this.Ships = new List<Ship>();
        }

        public string Name { get; set; }

        public PublicFieldStates[,] PublicBoard { get; set; }

        public IEnumerable<Ship> Ships { get; set; } 
    }
}
