using MEMORY.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Xml.Linq;

namespace MEMORY.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Game()
        {
            return RedirectToAction("CreateGame");
        }

        [HttpGet]
        public IActionResult JoinGame(string roomCode)
        {
            //Console.WriteLine("ROOMCODE FROM FORM: " + roomCode);

            if (string.IsNullOrWhiteSpace(roomCode))
                return BadRequest("Missing room code.");

            DatabaseMethods dbm = new DatabaseMethods();
            Game game = dbm.GetGameFromRoomCode(roomCode);

            if (game == null)
                return NotFound($"Game with room code '{roomCode}' not found.");

            return RedirectToAction("Game", new { roomCode });
        }
              
        public IActionResult CreateGame ()
        {
            Game game = new Game();
            //Round round = new Round();

            //slumpa fram rumid
            game.RoomCode = GenerateRoomCode();

            game.AmountOfPairs = 0;
            game.Player1 = 0;//dbm.GetUser(game) //när vi fixat inloggning, sätt till inloggad användare
            game.Player2 = null;
            game.CreatedWhen = DateTime.Now;
            game.CurrentPlayer = game.Player1; //börja med spelare 1
            game.State = GameState.Pending;

            //skapa alla kort

            //duplicera korten för att få par
            List<Card> cards = new List<Card>();
            for(int i = 0; i < cards.Count; i++)
            {
                cards.Add(new Card(cards[i]));
            }

            //slumpa kortens positioner
            cards = cards.OrderBy(cards => Random.Shared.Next()).ToList();

            //spara spelet i databasen
            DatabaseMethods dbm = new DatabaseMethods();
            //dbm.InsertGame(game);

            //spara korten i databasen
            //dbm.InsertCardList(cards, dbm.GetGameFromRoomCode(game.RoomCode).GameID);

            dbm.InsertGame(game); // sparar spelet

            Game gameId = dbm.GetGameFromRoomCode(game.RoomCode);

            dbm.InsertCardList(cards, game.GameID);

            return RedirectToAction("Game", new { roomCode = game.RoomCode });

            //redirecta till Game action
            //return RedirectToAction("Game", new { roomCode = game.RoomCode }); // rätt syntax för route values
        }
        public IActionResult Game(string roomCode)
        {
            if (string.IsNullOrWhiteSpace(roomCode))
                return BadRequest("Missing room code.");

            var dbm = new DatabaseMethods();
            Game game = dbm.GetGameFromRoomCode(roomCode);
            if (game == null)
                return NotFound($"Game with room code '{roomCode}' not found.");

            List<Card> cards = dbm.GetCardsFromGameID(game.GameID);

            GameViewModel viewModel = new GameViewModel
            {
                Game = game,
                Cards = cards
            };

            return View(viewModel);
        }

        public IActionResult GameOver(string roomCode, int gameID)
        {
            DatabaseMethods dbm = new DatabaseMethods();
            Game game = dbm.GetGameFromRoomCode(roomCode);
            int winner = dbm.GetWinner(gameID);
            ViewBag.Winner = winner;
            return View();
        }

        [HttpPost]
        public IActionResult SelectCard(int gameID, int index)
        {
            DatabaseMethods dbm = new DatabaseMethods();
            Card selectedCard = dbm.SelectCard(gameID, index);
            string roomCode = dbm.GetGameFromGameID(gameID).RoomCode;

            Round round = dbm.GetRoundFromGameID(gameID);

            //flippar kortet
            dbm.FlipCard(selectedCard);

            if (round.IsFirstCard())
            {
                round.SetCard1(selectedCard);
            }
            else
            {
                round.SetCard2(selectedCard);
                if (round.IsItAMatch())
                {
                    //Hantera match

                    //amount of pairs ++
                    dbm.IncreaseAmountOfPairs(gameID);

                    //låser korten face up
                    //matcha de två korten till player1 eller player2 (extra attribut i GameCard)

                    dbm.LockMatchedCards(round.card1, round.card2, (int)dbm.GetGameFromGameID(gameID).CurrentPlayer);
                    
                    //behöv detta?
                    ViewBag.Message = "It's a match!";

                }
                else
                {
                    // Hantera icke-match
                    //döljer korten igen (hide)
                    dbm.HideCardsAgain(round.card1, round.card2);

                    //byter spelare (SwitchPlayer)
                    dbm.SwitchPlayer(dbm.GetGameFromGameID(gameID));

                    //behövs detta?
                    ViewBag.Message = "Not a match. Next player's turn.";
                }

                dbm.EndOfRound(gameID);
                //end of round
                //amropar round.ResetRound(); i EndOfRound i databasmetoder
                //if AmountOfPairs == totalpairs -> EndOFGame (bl.a game.state = finished, jämföra vinnare, skriv ut vinnaren, radera korten från GameCard för dtr gameId)
                //om AmountOfPairs < totalpairs -> ny runda
            }
            if ((dbm.GetGameFromGameID(gameID).State == GameState.InProgress))
            {
                return RedirectToAction("Game", new { roomCode = dbm.GetGameFromRoomCode(roomCode).RoomCode });
            } else
            {
                int winnerID = dbm.GetWinner(gameID);
                return RedirectToAction("GameOver", new { roomCode = dbm.GetGameFromRoomCode(roomCode).RoomCode, winnerID = winnerID });
            }
        }

        private string GenerateRoomCode()
        {
            // Generate a random 6-character alphanumeric string
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
