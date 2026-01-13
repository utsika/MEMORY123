using MEMORY.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Xml.Linq;

namespace MEMORY.Controllers
{
    public class GameController : Controller
    {

              
        public IActionResult CreateGame ()
        {
            Game game = new Game();
            //Round round = new Round();

            //slumpa fram rumid
            game.RoomCode = GenerateRoomCode();

            game.AmountOfPairs = 0;
            game.Player1 = dbm.GetUser(game) //när vi fixat inloggning, sätt till inloggad användare
            game.Player2 = null;
            game.CreatedWhen = DateTime.Now;
            game.CurrentPlayer = game.Player1; //börja med spelare 1
            game.State = GameState.InProgress;

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
            dbm.InsertGame(game);

            //spara korten i databasen
            dbm.InsertCardList(cards, dbm.GetGameFromRoomCode(game.RoomCode).GameID);



            //redirecta till Game action
            return RedirectToAction("Game", {"RoomCode": game.RoomCode}); //objekt med roomcode, kolla upp
        }
        public IActionResult Game(string roomCode)
        {
            DatabaseMethods dbm = new DatabaseMethods();

            //hämta spelet från databasen
            Game game = dbm.GetGameFromRoomCode(roomCode);

            //hämta korten från databasen
            List<Card> cards = dbm.GetCardsFromGameID(game.GameID);

            return View(cards);
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
                //end of round
                //amropar round.ResetRound(); i EndOfRound i databasmetoder
                //if AmountOfPairs == totalpairs -> EndOFGame (bl.a game.state = finished, jämföra vinnare, skriv ut vinnaren, radera korten från GameCard för dtr gameId)
                //om AmountOfPairs < totalpairs -> ny runda
            }

            return RedirectToAction("Game", new { roomCode = dbm.GetGameFromRoomCode(roomCode).RoomCode });
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
