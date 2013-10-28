using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Battleships.Test
{
    [TestFixture]
    class ShipValidatorTest
    {
        private List<IList<Position>> input;

        [SetUp]
        public void SetUp()
        {
            input = new List<IList<Position>>()
                           {
                               new[]
                               {
                                   new Position(0, 0),
                                   new Position(0, 1),
                                   new Position(0, 2),
                                   new Position(0, 3),
                                   new Position(0, 4),
                               },
                               new []
                               {
                                   new Position(1, 6), 
                                   new Position(1, 7), 
                                   new Position(1, 8), 
                                   new Position(1, 9), 
                               },
                               new []
                               {
                                   new Position(5, 3), 
                                   new Position(5, 4), 
                                   new Position(5, 5), 
                                   new Position(5, 6), 
                               }, 
                               new []
                               {
                                   new Position(6, 1), 
                                   new Position(7, 1), 
                                   new Position(8, 1), 
                               },
                               new []
                               {
                                   new Position(4, 8), 
                                   new Position(5, 8), 
                                   new Position(6, 8), 
                               },
                               new []
                               {
                                   new Position(9, 7), 
                                   new Position(9, 8), 
                                   new Position(9, 9), 
                               },
                               new []
                               {
                                   new Position(1, 0), 
                                   new Position(1, 1), 
                               },
                               new []
                               {
                                   new Position(3, 0), 
                                   new Position(3, 1), 
                               },
                               new []
                               {
                                   new Position(3, 3), 
                                   new Position(4, 3), 
                               },
                               new []
                               {
                                   new Position(9, 4), 
                                   new Position(9, 5), 
                               }
                           };
        }
        [Test]
        public void WhenShipsAreOverlapping_ThenThrowArgumentException()
        {
            // ship overlaps with another ship
            this.input[8] = new[]
                                   {
                                       new Position(4, 3),
                                       new Position(5, 3),
                                   };

            Action action = () => ShipValidator.ValidateShips(this.input);

            action.ShouldThrow<ArgumentException>().WithMessage("position [5,3] is used multiple times");
        }

        [Test]
        public void WhenLessThan10ShipsDefined_ThenThrowArgumentException()
        {
            Action action = () => ShipValidator.ValidateShips(input.Skip(1));

            action.ShouldThrow<ArgumentException>().WithMessage("provided 9 instead of 10 ships");
        }

        [Test]
        public void WhenMoreThan10ShipsDefined_ThenThrowArgumentException()
        {
            this.input.Add(new[] { new Position(8, 8) });

            Action action = () => ShipValidator.ValidateShips(this.input);

            action.ShouldThrow<ArgumentException>().WithMessage("provided 11 instead of 10 ships");
        }

        [Test]
        public void WhenShipCoordinatesExceedBoundaries()
        {
            this.input[1][0] = new Position(1, 10);

            Action action = () => ShipValidator.ValidateShips(this.input);

            action.ShouldThrow<ArgumentException>().WithMessage("not all ships are within the allowed boundaries (0-9)");
        }

        [Test]
        public void WhenCoordinatesOfShipAreNotValid_ThenThrowArgumentException()
        {
            this.input[0] = new[]
                                   {
                                       new Position(0, 0),
                                       new Position(0, 1),
                                       new Position(0, 3),
                                       new Position(0, 4),
                                       new Position(0, 5),
                                   };

            Action action = () => ShipValidator.ValidateShips(this.input);

            action.ShouldThrow<ArgumentException>().WithMessage("ship has an invalid set of coordinates: [0,0],[0,1],[0,3],[0,4],[0,5]");
        }

        [Test]
        public void WhenIncorrectShipsProvied_ThenThrowArgumentException()
        {
            this.input[2] = new[]
                            {
                                new Position(7, 7),
                            };

            Action action = () => ShipValidator.ValidateShips(this.input);

            action.ShouldThrow<ArgumentException>()
                .WithMessage("Expected the following ships: 1x 5, 2x 4, 3x 3, 4x 2. But Received 1x 5, 1x 4, 3x 3, 4x 2, 1x 1");
        }
    }
}
