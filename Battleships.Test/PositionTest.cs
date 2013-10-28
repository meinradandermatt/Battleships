using FluentAssertions;
using NUnit.Framework;

namespace Battleships.Test
{
    [TestFixture]
    public class PositionTest
    {
        [Test]
        public void WhenSameCoordinateValues_ShouldBeEqual()
        {
            Position pos1 = new Position(){ X = 42, Y = 21 };
            Position pos2 = new Position(){ X = 42, Y = 21 };

            pos1.Should().Be(pos2);
        }

        [Test]
        public void WhenNotSameCoordinateValues_ShouldNotBeEqual()
        {
            Position pos1 = new Position() { X = 42, Y = 40 };
            Position pos2 = new Position() { X = 21, Y = 20 };

            pos1.Should().NotBe(pos2);
        }
    }
}
