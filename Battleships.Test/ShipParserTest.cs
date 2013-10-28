using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Battleships.Test
{
    [TestFixture]
    public class ShipParserTest
    {
        [Test]
        public void WhenShipContainsCoordinates_ThenReturnParsedShip()
        {
            var ships = ShipParser.Parse(new[]
                                         {
                                             new[]
                                             {
                                                 new Position(42, 21),
                                             }
                                         });

            ships.Should().HaveCount(1);
            ships.Single().Positions.Should().ContainSingle(p => p.X == 42 && p.Y == 21);
        }

        [Test]
        public void WhenShipsContainNoCoordinatesThenReturnNoShip()
        {
            var ships = ShipParser.Parse(new[]
                                         {
                                             new Position[] 
                                             {
                                             }
                                         });

            ships.Should().HaveCount(0);
        }
    }
}
