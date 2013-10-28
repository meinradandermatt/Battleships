using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Battleships.Test
{
    [TestFixture]
    public class GameFactoryTest
    {
        private IList<IList<Position>> player1Ships;
        private IList<IList<Position>> player2Ships;

        [SetUp]
        public void SetUp()
        {
            player1Ships = new List<IList<Position>>()
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

            player2Ships = player1Ships;
        }

        [Test]
        public void Create_ShouldInitializeEachPlayerWithEmptyPublicBoard()
        {
            var game = GameFactory.Create("1", player1Ships,"2", player2Ships);

            game.Player1.PublicBoard.Length.Should().Be(100);
            game.Player2.PublicBoard.Length.Should().Be(100);

            game.Player1.PublicBoard.Cast<PublicFieldStates>()
                .Where(f => f != PublicFieldStates.Unknown)
                .Should()
                .BeEmpty();

            game.Player2.PublicBoard.Cast<PublicFieldStates>()
                .Where(f => f != PublicFieldStates.Unknown)
                .Should()
                .BeEmpty();
        }

        [Test]
        public void Create_ShouldInitializeEachPlayerWithName()
        {
            var game = GameFactory.Create("1", player1Ships, "2", player2Ships);

            game.Player1.Name.Should().Be("1");
            game.Player2.Name.Should().Be("2");
        }

        [Test]
        public void Create_ShouldCreateShipsForSpecifiedCoordinates()
        {
            var game = GameFactory.Create("1", player1Ships, "2", player2Ships);

            game.Player1.Ships.Should().HaveCount(10);
            game.Player2.Ships.Should().HaveCount(10);
        }

        [Test]
        public void Create_WinnerShouldNotBeSet()
        {
            var game = GameFactory.Create("1", player1Ships, "2", player2Ships);

            game.Finished.Should().Be(false);
        }
    }
}