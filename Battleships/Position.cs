namespace Battleships
{
    public struct Position
    {
        public Position(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}