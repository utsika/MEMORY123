using MEMORY.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace MEMORY.Controllers
{
    public class GameController : Controller
    {

        private Game game = new Game();
        private Round round = new Round();
        private Card card = new Card();
        public IActionResult Game()
        {
            // ish så som det kan se ut
            // detta ska komma från databasen sen
            // hämtar kort från biblioteket
            // vet inte hur?????
            List<string> cards = new List<string>
            {
                "Card1",
                "Card2",
                "Card3",
                "Card4",
                "Card5",
                "Card6",
                "Card7",
                "Card8",
                "Card9",
                "Card10",
                "Card11",
                "Card12"
            };

            // prints number of cards
            int i = cards.Count;
            ViewBag.noofCards = i;

            return View(cards);
        }

        [HttpPost]
        public IActionResult SelectCard(Card card)
        {

            if (round.IsFirstCard())
            {
                round.SetCard1(card);
            }
            else
            {
                round.SetCard2(card);
                if (round.IsItAMatch())
                {
                    // Hantera match
                    ViewBag.Message = "It's a match!";

                }
                else
                {
                    // Hantera icke-match
                    ViewBag.Message = "Not a match. Next player's turn.";
                }

                round.ResetRound();
            }
            // vad vill vi visa????
            // vill visa listan över våra kort, väntar med detta tills vi har lagt in korten
            return View();
        }


    }
}
