using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Battleships.Test
{
    [TestFixture]
    public class BattleshipGameTest
    {
        private Player player1;
        private Player player2;

        private BattleshipGame testee;

        [SetUp]
        public void SetUp()
        {
            this.player1 = new Player();
            this.player2 = new Player();

            this.testee = new BattleshipGame(this.player1, this.player2);
        }

        [Test]
        public void NewGame_Player1ShouldHaveFirstMove()
        {
            this.testee.CurrentPlayer.Should().BeSameAs(this.testee.Player1);
        }

        [Test]
        public void MakeMove_WhenSpecifiedPlayerIsCurrentPlayer_ThenReturnPositionOfEnemyPublicField()
        {
            this.player2.Ships = new[] {new Ship(new[] {new Position(2, 2), new Position(2, 1) })};

            PublicFieldStates result = this.testee.MakeMove(2, 1);

            result.Should().Be(PublicFieldStates.Hit);
            this.player2.PublicBoard[1, 2].Should().Be(PublicFieldStates.Hit);
        }

        [Test]
        public void MakeMove_WhenPlayerMadeMove_ShouldChangeCurrentPlayer()
        {
            this.player1.Ships = new[] { new Ship(new[] { new Position(2, 2), new Position(2, 1) }) };
            this.player2.Ships = new[] { new Ship(new[] { new Position(2, 2), new Position(2, 1) }) };

            this.testee.MakeMove(2, 2);

            this.testee.CurrentPlayer.Should().BeSameAs(player2);

            this.testee.MakeMove(3, 3);

            this.testee.CurrentPlayer.Should().BeSameAs(player1);
        }

        [Test]
        public void MakeMove_WhenLastMoveSankEnemyShip_ThenSetShipToSunk()
        {
            this.player1.Ships = new[]
                                 {
                                     new Ship(new[]
                                              {
                                                  new Position(9, 9)
                                              }),
                                 };
            this.player2.Ships = new[]
                                 {
                                     new Ship(new[]
                                              {
                                                  new Position(4, 4),
                                                  new Position(4, 5),
                                                  new Position(4, 6)
                                              }),
                                     new Ship(new[]
                                         {
                                             new Position(9, 9),
                                         })
                                 };

            var move1 = this.testee.MakeMove(4, 4);
            this.testee.MakeMove(1, 1);
            var move2 = this.testee.MakeMove(4, 6);
            this.testee.MakeMove(1, 1);
            var move3 = this.testee.MakeMove(4, 5);

            move1.Should().Be(PublicFieldStates.Hit);
            move2.Should().Be(PublicFieldStates.Hit);
            move3.Should().Be(PublicFieldStates.Sunk);

            this.player2.PublicBoard[4, 4].Should().Be(PublicFieldStates.Sunk);
            this.player2.PublicBoard[5, 4].Should().Be(PublicFieldStates.Sunk);
            this.player2.PublicBoard[6, 4].Should().Be(PublicFieldStates.Sunk);

            this.player2.Ships.Single(s => s.Sunk).Sunk.Should().BeTrue();
        }

        [Test]
        public void MakeMove_WhenMoveSankLastRemainingEnemyShip_ThenPlayerWinsGame()
        {
            this.player2.Ships = new[]
                                 {
                                     new Ship(new[]
                                              {
                                                  new Position(4, 4),
                                                  new Position(4, 5),
                                                  new Position(4, 6)
                                              }),
                                 };
            this.player2.PublicBoard[4,4] = PublicFieldStates.Hit;
            this.player2.PublicBoard[5,4] = PublicFieldStates.Hit;

            PublicFieldStates result = this.testee.MakeMove(4, 6);

            result.Should().Be(PublicFieldStates.Sunk);

            this.testee.Finished.Should().BeTrue();
            this.testee.CurrentPlayer.Should().BeSameAs(this.player1);
        }

        [Test]
        public void MakeMove_WhenGameFinished_ThenThrowInvalidOperationException()
        {
            this.testee.Finished = true;

            Action action = () => this.testee.MakeMove(1, 1);

            action.ShouldThrow<InvalidOperationException>().WithMessage("This game has ended");
        }
    }
}