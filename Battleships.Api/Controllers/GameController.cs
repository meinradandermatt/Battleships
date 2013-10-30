using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Battleships.Api.Hubs;
using Microsoft.AspNet.SignalR;

namespace Battleships.Api.Controllers
{
    //TODO exception when player with same name joins multiple games
    //TODO ensure only current player can make move
    //TODO notify users when current user changes

    [RoutePrefix("game")]
    public class GameController : ApiController
    {
        private static readonly IDictionary<Guid, BattleshipGame> games = new Dictionary<Guid, BattleshipGame>();
        private static readonly Queue<JoinGameModel> playerQueue = new Queue<JoinGameModel>();
        private static readonly object lockObj = new object();

        private static readonly IHubContext gameHubContext = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

        [Route("{gameId}")]
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
                                         new PlayerData()
                                         {
                                             Name = game.Player1.Name,
                                             Board = game.Player1.PublicBoard
                                         }, 
                                         new PlayerData()
                                         {
                                             Name = game.Player2.Name,
                                             Board = game.Player2.PublicBoard
                                         }, 
                                     }
                };
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        [Route("{gameId}/{enemyName}/board/{x};{y}")]
        [HttpPut]
        public PublicFieldStates BombField(Guid gameId, string enemyName, int x, int y)
        {
            if (games.ContainsKey(gameId))
            {
                var game = games[gameId];
                if (game.Player1.Name != enemyName && game.Player2.Name != enemyName)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content =
                            new StringContent(
                            string.Format("Game contains no player with name '{0}'",
                                enemyName))
                    });
                }

                if (game.CurrentPlayer.Name == enemyName)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Not your turn")
                    });
                }

                var result = game.MakeMove(x, y);
                gameHubContext.Clients.All.update(gameId);
                return result;
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
        public GameAccessData JoinGame(JoinGameModel joinGameModel)
        {
            ShipValidator.ValidateShips(joinGameModel.Ships);

            if (string.IsNullOrEmpty(joinGameModel.Name))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            bool waiting = false;
            JoinGameModel enemy = null;
            lock (lockObj)
            {
                // check if an enemy is available, otherwise enqueue in waiting list
                if (playerQueue.Count > 0)
                {
                    enemy = playerQueue.Dequeue();
                }
                else
                {
                    playerQueue.Enqueue(joinGameModel);
                    waiting = true;
                }
            }

            if (waiting)
            {
                // wait to be notified when the player has been added to a game
                joinGameModel.ResetEvent.WaitOne();
                var game = games
                    .Where(g => g.Value.Player1.Name == joinGameModel.Name || g.Value.Player2.Name == joinGameModel.Name)
                    .Last();
                return new GameAccessData() { GameId = game.Key };
            }
            else
            {
                // create a new game and notify the other player
                try
                {
                    var game = GameFactory.Create(enemy.Name, enemy.Ships, joinGameModel.Name, joinGameModel.Ships);
                    var gameId = Guid.NewGuid();
                    games.Add(gameId, game);
                    enemy.ResetEvent.Set();
                    return new GameAccessData() { GameId = gameId };
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

    public class JoinGameModel
    {
        public string Name { get; set; }
        public IEnumerable<IEnumerable<Position>> Ships { get; set; }
        public AutoResetEvent ResetEvent = new AutoResetEvent(false);
    }

    public class GameResourceData
    {
        public bool IsRunning { get; set; }
        public string CurrentPlayer { get; set; }
        public IEnumerable<PlayerData> Players { get; set; }
    }

    public class PlayerData
    {
        public string Name { get; set; }
        public PublicFieldStates[,] Board { get; set; }
    }

    public class GameAccessData
    {
        public Guid GameId { get; set; }
        public Guid Secret { get; set; }
    }


}
