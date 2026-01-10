using MEMORY.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MEMORY.Controllers
{
    public class GameController : Controller
    {

              
        public IActionResult CreateGame ()
        {
            Game game = new Game();

            //slumpa fram rumid
            game.RoomCode = GenerateRoomID();
            game.AmountOfPairs = 0;
            game.Player1 = 1; //när vi fixat inloggning, sätt till inloggad användare
            game.Player2 = null;
            game.CreatedWhen = DateTime.Now;

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
            Game game = dbm.GetGameFromRoomCode(roomCode);

            //hämta korten från databasen
            List<Card> cards = dbm.GetCardsFromGameID(game.GameID);

            return View(cards);
        }

       

        [HttpPost]
        public IActionResult SelectCard(int index, int gameID)
        {
            DatabaseMethods dbm = new DatabaseMethods();
            dbm.SelectCard(index, gameID);

            
            return RedirectToAction("Game", new { roomCode = dbm.GetGameFromRoomCode(gameID).RoomID });

        }

        private string GenerateRoomID()
        {
            // Generate a random 6-character alphanumeric string
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
