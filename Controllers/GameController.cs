using MEMORY.Hubs;
using MEMORY.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace MEMORY.Controllers
{

	public class GameController : Controller
	{

        private readonly IHubContext<GameHub> hubContext1;

        public GameController(IHubContext<GameHub> hubContext)
        {
            hubContext1 = hubContext;
        }
		
		[HttpPost]
		public IActionResult JoinGame(string roomCode)
		{

			if (string.IsNullOrWhiteSpace(roomCode))
				return BadRequest("Missing room code.");

			DatabaseMethods dbm = new DatabaseMethods();
			Game game = dbm.GetGameFromRoomCode(roomCode);
			User currentUser = HttpContext.Session.GetObject<User>("currentUser");
			if (currentUser == null)
				return RedirectToAction("Login", "User");


			if (game == null)
				return NotFound($"Spelet med koden '{roomCode}' finns inte.");

			if (game.Player2 != null)
			{ 
				ModelState.AddModelError("", "Spelet har redan två spelare");
				return RedirectToAction("Index", "Home");
			}

            if (game.Player1 == currentUser.UserID)
            {
				ModelState.AddModelError("", "Du är redan spelare 1");
				return RedirectToAction("Game", new { roomCode });
			}

			dbm.SetPlayer2(game.GameID, currentUser.UserID);

			dbm.UpdateGameState(game.GameID, GameState.InProgress);


            return RedirectToAction("Game", new { roomCode });

        }
		[HttpGet]
		public IActionResult CreateGame()
		{
			DatabaseMethods dbm = new DatabaseMethods();

			Game game = new Game();

            User player1 = HttpContext.Session.GetObject<User>("currentUser");          
			
			//slumpa fram rumid
            game.RoomCode = GenerateRoomCode();

			game.AmountOfPairs = 0;
			game.Player1 = player1.UserID;
			game.Player2 = null;
			game.CreatedWhen = DateTime.Now;
			game.CurrentPlayer = game.Player1; //börja med spelare 1
			game.State = GameState.Pending;


			//skapa alla kort
			List<Card> cards = dbm.GetCardsFromCardLibrary();

			List<Card> allcards = new List<Card>();
			//duplicera korten för att få par
			for (int i = 0; i < cards.Count; i++)
			{
				allcards.Add(new Card(cards[i]));
				allcards.Add(new Card(cards[i]));
			}

			//slumpa kortens positioner
			allcards = allcards.OrderBy(allcards => Random.Shared.Next()).ToList();

			//Saves the game in the database
			dbm.InsertGame(game); 

			string hej = game.RoomCode;

			Game gamehej = dbm.GetGameFromRoomCode(hej);

			int id = gamehej.GameID;

			dbm.CreateAndInsertNewRound(id);

			dbm.InsertCardList(allcards, dbm.GetGameFromRoomCode(game.RoomCode).GameID);

			return RedirectToAction("Game", new { roomCode = game.RoomCode });
		}


        public async Task<IActionResult> Game(string roomCode)
		{
			if (string.IsNullOrWhiteSpace(roomCode))
				return BadRequest("Missing room code.");

			var dbm = new DatabaseMethods();
			Game game = dbm.GetGameFromRoomCode(roomCode);
			if (game == null)
				return NotFound($"Game with room code '{roomCode}' not found.");

			List<Card> cards = dbm.GetCardsFromGameID(game.GameID);

			Round round = dbm.GetRoundFromGameID(game.GameID);

            if (round.IndexCard1 != null && round.IndexCard2 != null)
			{
				bool isMatch = dbm.DetermineMatch((int)round.IndexCard1, (int)round.IndexCard2, game.GameID);

				if (isMatch)
				{
					dbm.IncreaseAmountOfPairs(game.GameID);
					dbm.LockMatchedCards((int)round.IndexCard1, (int)round.IndexCard2, game.CurrentPlayer);
				}
				else
				{
					dbm.HideCardsAgain((int)round.IndexCard1, (int)round.IndexCard2);
                    
                    int nextPlayer = game.CurrentPlayer == game.Player1 ? game.Player2.Value : game.Player1;

                    dbm.UpdateCurrentPlayer(game.GameID, nextPlayer);
                }

				dbm.EndOfRound(game.GameID);
			}

			int pairs = dbm.GetAmountOfPairs(game.GameID);

            if (pairs == 9)
            {
                dbm.EndGame(game.GameID);
				Game endedGame = dbm.GetGameFromGameID(game.GameID);
                
                return RedirectToAction("GameOver", new { winnerID = endedGame.Winner }); 	
            }

            GameViewModel viewModel = new GameViewModel
			{
				Game = game,
				Cards = cards
			};

			return View(viewModel);
		}

		public IActionResult GameOver( int winnerID) 
		{

            DatabaseMethods dbm = new DatabaseMethods();

            // Get the name of the winner
            string winnerName = dbm.GetUserByID(winnerID).UserName;

            // Send a view model with the name of the winner
            var model = new GameViewModel
            {
                WinnerName = winnerName,
            };
            return View(model);
        }

		//private void SetPlayersName(int player1ID, int player2ID)
		//{
		//	DatabaseMethods dbm = new DatabaseMethods();
		//	string player1Name = dbm.GetUserByID(player1ID).UserName;
		//	string player2Name = dbm.GetUserByID((int)player2ID).UserName;

		//	var model = new GameViewModel
		//	{
		//		Player1Name = player1Name,
		//		Player2Name = player2Name,
		//	};

		//}

		[HttpGet]
        public async Task<IActionResult> SelectCard(int gameID, int index)
        {
			DatabaseMethods dbm = new DatabaseMethods();
			Card selectedCard = dbm.SelectCard(gameID, index);
            Game game = dbm.GetGameFromGameID(gameID);
			   
            User currentUser = HttpContext.Session.GetObject<User>("currentUser");

			if(game.State == GameState.Pending)
			{
				return RedirectToAction("Game", new { roomCode = game.RoomCode });
			}

            if (currentUser == null)
                return RedirectToAction("Login", "User");

            if (game.CurrentPlayer != currentUser.UserID)
            {
                //Stops the user from playing if it isn't their turn
                return RedirectToAction("Game", new { roomCode = game.RoomCode });
            }

			string roomCode = dbm.GetGameFromGameID(gameID).RoomCode;

			Round round = dbm.GetRoundFromGameID(gameID);

			int cardIndex = selectedCard.Index;

			dbm.FlipCard(index);

            await hubContext1.Clients
			.Group(roomCode)
			.SendAsync("RefreshGame");

            if (round.IndexCard1 == null)
			{
				dbm.InsertCard1IntoRound(gameID, index);
			}
			else
			{
				dbm.InsertCard2IntoRound(gameID, index);
			}
			
			if (dbm.GetGameFromGameID(gameID).State == GameState.InProgress)
			{
				return RedirectToAction("Game", new { roomCode = dbm.GetGameFromRoomCode(roomCode).RoomCode });
			}	
			else
			{
				int winnerID = dbm.GetWinner(gameID);
                return RedirectToAction("GameOver", new { roomCode = roomCode, winnerID = winnerID });
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
