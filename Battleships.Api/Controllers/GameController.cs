using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Battleships.Api.Controllers
{
    //TODO exception when player with same name joins multiple games
    //TODO ensure only current player can make move
    //TODO notify users when current user changes

    [RoutePrefix("game")]
    public class GameController : ApiController
    {
        private static readonly IDictionary<Guid, BattleshipGame> games = new Dictionary<Guid, BattleshipGame>();
        private static readonly Queue<PlayerData> playerQueue = new Queue<PlayerData>();
        private static readonly object lockObj = new object();

        [Route("")]
        [HttpGet]
        public GameResourceData GetGame(Guid gameId)
        {
            if (games.ContainsKey(gameId))
            {
                BattleshipGame game = games[gameId];
                return new GameResourceData()
                       {
                           CurrentPlayer = game.CurrentPlayer.Name,
                           IsRunning = false,
                           Players = new[]
                                     {
                                         game.Player1.Name,
                                         game.Player2.Name
                                     }
                       };
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [Route("{gameId}/players/{enemyName}/board/{x};{y}")]
        [HttpPut]
        public PublicFieldStates BombField(Guid gameId, string enemyName, int x, int y)
        {
            if (games.ContainsKey(gameId))
            {
                var game = games[gameId];
                if (game.Player1.Name != enemyName || game.Player2.Name != enemyName)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                                                    {
                                                        Content =
                                                            new StringContent(
                                                            string.Format("Game contains no player with name '{0}'",
                                                                enemyName))
                                                    });
                }

                var enemy = game.Player1.Name == enemyName ? game.Player1 : game.Player2;
                if (enemy.Name == enemyName)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                                                    {
                                                        Content = new StringContent("Not your turn")
                                                    });
                }

                return game.MakeMove(x, y);
            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                                            {
                                                Content =
                                                    new StringContent(string.Format("No game with id '{0}' found",
                                                        gameId))
                                            });
        }

        [Route("")]
        [HttpPost]
        public string JoinGame(PlayerData playerData)
        {
            ShipValidator.ValidateShips(playerData.Ships);

            if (string.IsNullOrEmpty(playerData.Name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            bool waiting = false;
            PlayerData enemy = null;
            lock (lockObj)
            {
                // check if an enemy is available, otherwise enqueue in waiting list
                if (playerQueue.Count > 0)
                {
                    enemy = playerQueue.Dequeue();
                }
                else
                {
                    playerQueue.Enqueue(playerData);
                    waiting = true;
                }
            }

            if (waiting)
            {
                // wait to be notified when the player has been added to a game
                playerData.ResetEvent.WaitOne();
                var game = games
                    .Where(g => g.Value.Player1.Name == playerData.Name || g.Value.Player2.Name == playerData.Name)
                    .Last();
                return game.Key.ToString();
            }
            else
            {
                // create a new game and notify the other player
                try
                {
                    var game = GameFactory.Create(enemy.Name, enemy.Ships, playerData.Name, playerData.Ships);
                    var gameId = Guid.NewGuid();
                    games.Add(gameId, game);
                    enemy.ResetEvent.Set();
                    return gameId.ToString();
                }
                catch (Exception)
                {
                    lock (lockObj)
                    {
                        playerQueue.Enqueue(enemy);
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }

    public class PlayerData
    {
        public string Name { get; set; }
        public IEnumerable<IEnumerable<Position>> Ships { get; set; }
        public AutoResetEvent ResetEvent = new AutoResetEvent(false);
    }

    public class GameResourceData
    {
        public bool IsRunning { get; set; }
        public string CurrentPlayer { get; set; }
        public IEnumerable<string> Players { get; set; }
    }

    
}
