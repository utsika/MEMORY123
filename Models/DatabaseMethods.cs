using MEMORY.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Reflection;

namespace MEMORY.Models
{
    public class DatabaseMethods
    {
        

        public Card GetCardByIndex(int gameID, int index)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"SELECT * FROM GameCard 
             WHERE GameID = @gameId AND [Index] = @index", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@gameID", gameID);
            cmd.Parameters.AddWithValue("@index", index);

            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            //returns the retrieved carddetails
            return new Card
            {
                CardID = (int)reader["CardID"],
                CardName = reader["CardName"].ToString()
            };
        }


        /// <summary>
        /// Inserts a new game into the database
        /// </summary>
        /// <param name="game">The current game</param>
        public void InsertGame(Game game)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"INSERT INTO Game
             (CreatedWhen, Player1, Player2, CurrentPlayer, RoomCode, State, AmountOfPairs, Winner)
             VALUES
             (@createdWhen, @player1, @player2, @currentPlayer, @roomCode, @state, @amountOfPairs, @winner)",
             sqlConnection);

            //sets the parameters for the SQL command
            //detta gäller för alla attribut i Game klassen
            cmd.Parameters.AddWithValue("@gameID", game.GameID);
            cmd.Parameters.Add("@createdWhen", SqlDbType.DateTime).Value = game.CreatedWhen;
            cmd.Parameters.Add("@player1", SqlDbType.Int).Value = game.Player1;
            cmd.Parameters.Add("@player2", SqlDbType.Int).Value = (object)game.Player2 ?? DBNull.Value;
            cmd.Parameters.Add("@currentPlayer", SqlDbType.Int).Value = (object)game.CurrentPlayer ?? DBNull.Value;
            cmd.Parameters.Add("@roomCode", SqlDbType.NVarChar, 50).Value = (object)game.RoomCode ?? DBNull.Value;
            cmd.Parameters.Add("@state", SqlDbType.Int).Value = (int)game.State;
            cmd.Parameters.Add("@amountOfPairs", SqlDbType.Int).Value = game.AmountOfPairs;
            cmd.Parameters.Add("@winner", SqlDbType.Int).Value = (object)game.Winner ?? DBNull.Value;

            int rows = ExecuteNonQuery(sqlConnection, cmd);
            if (rows != 1)
            {
                // Hantera fel
                throw new Exception("Failed to insert game");
            }
        }

        public List<Card> GetCardsFromCardLibrary()
        {
            List<Card> cards = new List<Card>();

            SqlConnection sqlConnection = CreateSQLConnection();

            //hämta kort från CardLibrary
            using SqlCommand cmd = new SqlCommand(
                @"SELECT * FROM CardLibrary", sqlConnection);

            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);
            
            while (reader.Read())
            {
                Card card = new Card();
                card.CardID = (int)reader["CardID"];
                card.CardName = reader["CardName"].ToString();

                cards.Add(card);
            }

            return cards;
        }

        public void SetPlayer1(int gameID, int playerID)
        {
            using SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
                @"UPDATE Game
              SET Player1 = @player1
              WHERE GameID = @gameId", sqlConnection);

            cmd.Parameters.AddWithValue("@player1", playerID);
            cmd.Parameters.AddWithValue("@gameId", gameID);

            ExecuteNonQuery(sqlConnection, cmd);
        }

        public void SetPlayer2 (int gameID, int playerID)
        {
            using SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
                @"UPDATE Game
              SET Player2 = @player2
              WHERE GameID = @gameId", sqlConnection);

            cmd.Parameters.AddWithValue("@player2", playerID);
            cmd.Parameters.AddWithValue("@gameId", gameID);

            ExecuteNonQuery(sqlConnection, cmd);
        }

        public void UpdateCurrentPlayer(int gameID, int playerID)
        {
            using SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
                "UPDATE Game SET CurrentPlayer = @player WHERE GameID = @id",
                sqlConnection);

            cmd.Parameters.AddWithValue("@player", playerID);
            cmd.Parameters.AddWithValue("@id", gameID);

            ExecuteNonQuery(sqlConnection, cmd);
        }

        /// <summary>
        /// Inserts a list of cards into the database for a specific game
        /// </summary>
        /// <param name="cards">The list of cards colleced from the CardLibrary</param>
        /// <param name="gameID">The ID of the current game</param>
        public void InsertCardList(List<Card> cards, int gameID)
        {
            foreach (Card card in cards)
            {
                SqlConnection sqlConnection = CreateSQLConnection();

                using SqlCommand cmd = new SqlCommand(
                 @"INSERT INTO GameCard
                (GameID, CardID, CardName, IsMatched, IsFlipped, PlayerMatchedTo)
                VALUES
                (@gameID, @cardID, @cardName, @isMatched, @isFlipped, @playerMatchedTo)",
                 sqlConnection);

                cmd.Parameters.AddWithValue("@gameID", gameID);
                cmd.Parameters.AddWithValue("@cardID", card.CardID);
                cmd.Parameters.AddWithValue("@cardName", card.CardName);
                cmd.Parameters.AddWithValue("@isMatched", card.IsMatched);
                cmd.Parameters.AddWithValue("@isFlipped", card.IsFlipped);
                cmd.Parameters.Add("@playerMatchedTo", SqlDbType.Int).Value = (object)card.PlayerMatchedTo ?? DBNull.Value;

                int rows = ExecuteNonQuery(sqlConnection, cmd);
                if (rows != 1)
                {
                    // Hantera fel
                    throw new Exception("Failed to insert cards");
                }

            }
        }

        /// <summary>
        /// Retrieves a specific game from the database by its room code
        /// </summary>
        /// <param name="roomCode">The roomCode for the game</param>
        /// <returns>The game connected to the roomCode</returns>
        public Game GetGameFromRoomCode(string roomCode)
        {
            //Game game = new Game();
            SqlConnection sqlConnection = CreateSQLConnection();

            //fixa i databas så allt stämmer överens!!!!
            using SqlCommand cmd = new SqlCommand(
             @"SELECT * FROM Game WHERE RoomCode = @roomCode", sqlConnection);
            cmd.Parameters.Add("@roomCode", SqlDbType.NVarChar, 50).Value = (object)roomCode ?? DBNull.Value;
                    
            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);

            if (!reader.Read())
                return null;

            Game game = new Game();

            //fylla game objektet med data från databasen
            game.GameID = (int)reader["GameID"];
            game.CreatedWhen = (DateTime)reader["CreatedWhen"];
            game.Player1 = (int)reader["Player1"];
            game.Player2 = reader["Player2"] as int?;
            game.CurrentPlayer = (int)reader["CurrentPlayer"];
            game.RoomCode = reader["RoomCode"].ToString();
            game.State = (GameState)Enum.Parse(typeof(GameState), reader["State"].ToString());
            game.AmountOfPairs = (int)reader["AmountOfPairs"];
            game.Winner = reader["Winner"] as int?;

            //returns the retrieved carddetails
            return game;
        }

        /// <summary>
        /// Retrieves all cards from a specific game by its gameID
        /// </summary>
        /// <param name="gameID">The ID of the current game</param>
        /// <returns>The list of the cards in the game</returns>
        public List<Card> GetCardsFromGameID (int gameID)
        {
            List<Card> cards = new List<Card>();

            SqlConnection sqlConnection = CreateSQLConnection();

            //fixa i databas så allt stämmer överens!!!!
            using SqlCommand cmd = new SqlCommand(
             @"SELECT * FROM GameCard 
             WHERE GameID = @gameID", sqlConnection);
         
            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@gameID", gameID);

            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);

            while (reader.Read())
            {
                Card card = new Card();
                card.CardID = (int)reader["CardID"];
                card.CardName = reader["CardName"].ToString();
                card.Index = (int)reader["Index"];
                card.IsMatched = (bool)reader["IsMatched"];
                card.IsFlipped = (bool)reader["IsFlipped"];
                card.PlayerMatchedTo = reader["PlayerMatchedTo"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["PlayerMatchedTo"]);


                cards.Add(card);
            }
           
            //returns the retrieved carddetails
            return cards;
        }


        /// <summary>
        /// Retrieves the selected card from the database
        /// </summary>
        /// <param name="gameID">The ID of the current game</param>
        /// <param name="index">The placement of the card</param>
        /// <returns>The selected card</returns>
        public Card SelectCard (int gameID, int index)
        {
            //hämta kortet som klickas från databasen
            Card selectedCard = GetCardByIndex(gameID, index);
            return selectedCard;
        }

		public void UpdateGameState(int gameID, Enum GameState){
			SqlConnection sqlConnection = CreateSQLConnection();
			using SqlCommand cmd = new SqlCommand(
			 @"UPDATE Game
			 SET State = @state
			 WHERE GameID = @gameID", sqlConnection);

			//sets the parameters for the SQL command
			cmd.Parameters.AddWithValue("@state", GameState);
			cmd.Parameters.AddWithValue("@gameID", gameID);

			ExecuteNonQuery(sqlConnection, cmd);
			
		}

        public Boolean DetermineMatch(int indexCard1, int indexCard2, int gameID)
        {
            //Card card1 = GetCardByIndex(indexCard1);
            //Card card2 = GetCardByIndex(indexCard2);
            //return card1.CardName == card2.CardName;
            Card card1 = GetCardByIndex(gameID, indexCard1);
            Card card2 = GetCardByIndex(gameID, indexCard2);
            return card1.CardName == card2.CardName;
        }

        public Card GetCardByIndex(int cardIndex)
        {
            Card card = new Card();
            SqlConnection sqlConnection = CreateSQLConnection();

            //fixa i databas så allt stämmer överens!!!!
            using SqlCommand cmd = new SqlCommand(
             @"SELECT * FROM GameCard 
             WHERE [Index] = @index", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@index", cardIndex);

            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);
            if (!reader.Read())
                return card;

            //fylla game objektet med data från databasen
            card.CardID = (int)reader["CardID"];
            card.CardName = reader["CardName"].ToString();
            card.Index = (int)reader["Index"];
            card.IsMatched = (bool)reader["IsMatched"];
            card.IsFlipped = (bool)reader["IsFlipped"];
            card.PlayerMatchedTo = reader["PlayerMatchedTo"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["PlayerMatchedTo"]);

            //returns the retrieved carddetails
            return card;
        }

        /// <summary>
        /// Gets a specific game from the database by its gameID
        /// </summary>
        /// <param name="gameID">The ID of the current game</param>
        /// <returns>The game as an object</returns>

        public Game GetGameFromGameID (int gameID)
        {
            Game game = new Game();
            SqlConnection sqlConnection = CreateSQLConnection();

            //fixa i databas så allt stämmer överens!!!!
            using SqlCommand cmd = new SqlCommand(
             @"SELECT * FROM Game 
             WHERE GameID = @gameID", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@gameID", gameID);

            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);
            if (!reader.Read())
                return game;

            //fylla game objektet med data från databasen
            game.GameID = (int)reader["GameID"];
            game.CreatedWhen = (DateTime)reader["CreatedWhen"];
            game.Player1 = (int)reader["Player1"];
            game.Player2 = reader["Player2"] as int?;
            game.CurrentPlayer = (int)reader["CurrentPlayer"];
            game.RoomCode = reader["RoomCode"].ToString();
            game.State = (GameState)Enum.Parse(typeof(GameState), reader["State"].ToString());

            game.AmountOfPairs = (int)reader["AmountOfPairs"];
            game.Winner = reader["Winner"] as int?;


            //returns the retrieved carddetails
            return game;
        }

        /// <summary>
        /// Gets the latest round from a specific game by its gameID
        /// </summary>
        /// <param name="gameID">The gameID for the current game</param>
        /// <returns>The current round</returns>

        public Round GetRoundFromGameID(int gameID)
        {
            Round round = new Round();
            SqlConnection sqlConnection = CreateSQLConnection();

            //retrieves the latest round for the specific game
            using SqlCommand cmd = new SqlCommand(
             @"SELECT TOP 1 * FROM Round 
             WHERE GameID = @gameID
             ORDER BY RoundID DESC", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@gameID", gameID);

            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);

            if (!reader.Read())
                return round;

            round.RoundID = (int)reader["RoundID"];
            round.IndexCard1 = reader["IndexCard1"] as int?;
            round.IndexCard2 = reader["IndexCard2"] as int?;
            round.WasItAMatch = (bool)reader["WasItAMatch"];

            //returns the retrieved rounddetails
            return round;
        }

        /// <summary>
        /// Switches the current player in the game
        /// If current player is player 1, switch to player 2 and vice versa
        /// </summary>
        /// <param name="game">Current game in progress</param>
        
        public void SwitchPlayer(Game game)
        {
            if (game.CurrentPlayer == game.Player1)
            {
                SqlConnection sqlConnection = CreateSQLConnection();

                using SqlCommand cmd = new SqlCommand(
                 @"UPDATE Game
                 SET CurrentPlayer = @player2
                 WHERE GameID = @gameID", sqlConnection);

                //sets the parameters for the SQL command
                cmd.Parameters.AddWithValue("@player2", game.Player2);
                cmd.Parameters.AddWithValue("@gameID", game.GameID);

                int rows = ExecuteNonQuery(sqlConnection, cmd);
                if (rows != 1)
                {
                    // Hantera fel, t.ex. kasta ett undantag eller logga
                    throw new Exception("Failed to execute the SQL command");
                }
            }
            else
            {
                //sql command to switch to player 1
                SqlConnection sqlConnection = CreateSQLConnection();

                using SqlCommand cmd = new SqlCommand(
                 @"UPDATE Game
                 SET CurrentPlayer = @player1
                 WHERE GameID = @gameID", sqlConnection);

                //sets the parameters for the SQL command
                cmd.Parameters.AddWithValue("@player1", game.Player1);
                cmd.Parameters.AddWithValue("@gameID", game.GameID);

                int rows = ExecuteNonQuery(sqlConnection, cmd);
                if (rows != 1)
                {
                    // Hantera fel, t.ex. kasta ett undantag eller logga
                    throw new Exception("Failed to execute the SQL command");
                }
            }
        }

        /// <summary>
        /// Hides the two selected cards again if they were not a match
        /// </summary>
        /// <param name="card1">The first selected card</param>
        /// <param name="card2">The second selected card</param>
        public void HideCardsAgain(int cardIndex1, int cardIndex2)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"UPDATE GameCard
             SET IsFlipped = 0
             WHERE [Index] = @cardIndex1 OR [Index] = @cardIndex2", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@cardIndex1", cardIndex1);
            cmd.Parameters.AddWithValue("@cardIndex2", cardIndex2);

            int rows = ExecuteNonQuery(sqlConnection, cmd);
        }

        /// <summary>
        /// Locks the two matched cards face up and assigns them to the player who matched them
        /// </summary>
        /// <param name="card1">The first selected card</param>
        /// <param name="card2">The second selected card</param>
        /// <param name="playerID">The player who made the match</param>
        public void LockMatchedCards(int cardIndex1, int cardIndex2, int playerID)
        {
            SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
             @"UPDATE GameCard
             SET IsMatched = 1, PlayerMatchedTo = @playerID
             WHERE [Index] = @cardIndex1 OR [Index] = @cardIndex2", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@cardIndex1", cardIndex1);
            cmd.Parameters.AddWithValue("@cardIndex2", cardIndex2);
            cmd.Parameters.AddWithValue("@playerID", playerID);

            int rows = ExecuteNonQuery(sqlConnection, cmd);
            if (rows != 2)
            {
                // Hantera fel, t.ex. kasta ett undantag eller logga
                throw new Exception("Failed to execute the SQL command");
            }
        }

        public void SetCard1 (int cardIndex, int gameID)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
            @"UPDATE Round
            SET IndexCard1 = @cardIndex1
            WHERE GameID = @gameID", sqlConnection);

            cmd.Parameters.AddWithValue("@cardIndex1", cardIndex);
            cmd.Parameters.AddWithValue("@gameID", gameID);

            int rows = ExecuteNonQuery(sqlConnection, cmd);
           
        }

        public void SetCard2(int cardIndex, int gameID)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
            @"UPDATE Round
            SET IndexCard2 = @cardIndex2
            WHERE GameID = @gameID", sqlConnection);

            cmd.Parameters.AddWithValue("@cardIndex2", cardIndex);
            cmd.Parameters.AddWithValue("@gameID", gameID);

            int rows = ExecuteNonQuery(sqlConnection, cmd);

        }


        /// <summary>
        /// Flips a selected card face up
        /// </summary>
        /// <param name="card">The selected card</param>
        public void FlipCard(int cardIndex)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"UPDATE GameCard
             SET IsFlipped = 1
             WHERE [Index] = @index", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@index", cardIndex);

            int rows = ExecuteNonQuery(sqlConnection, cmd);
            if (rows != 1)
            {
                // Hantera fel, t.ex. kasta ett undantag eller logga
                throw new Exception("Failed to execute the SQL command");
            }
        }

        /// <summary>
        /// Increases the amount of pairs in the game by 1, if a match is made
        /// </summary>
        /// <param name="gameID">The ID of the current game</param>
        /// <returns>The updated number of pairs</returns>
        public int IncreaseAmountOfPairs (int gameID)
        {
            int amountOfPairs = GetAmountOfPairs(gameID) + 1;

            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"UPDATE Game
             SET AmountOfPairs = @amountOfPairs
             WHERE GameID = @gameID", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@amountOfPairs", amountOfPairs);
            cmd.Parameters.AddWithValue("@gameID", gameID);

            int rows = ExecuteNonQuery(sqlConnection, cmd);
            if (rows != 1)
            {
                // Hantera fel, t.ex. kasta ett undantag eller logga
                throw new Exception("Failed to execute the SQL command");
            }
            return amountOfPairs;
        }

        /// <summary>
        /// Retrieves the number of pairs associated with the specified game.
        /// </summary>
        /// <param name="gameID">The ID of the current game</param>
        /// <returns>The number of pairs for the specified game. Returns 0 if the game does not exist or no pairs are found.</returns>
        public int GetAmountOfPairs (int gameID)
        {
            int amountOfPairs = 0;

            SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
             @"SELECT AmountOfPairs FROM Game 
             WHERE GameID = @gameID", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@gameID", gameID);
            
            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);
            if (!reader.Read())
                return amountOfPairs;
            amountOfPairs = (int)reader["AmountOfPairs"];
            return amountOfPairs;
        }

        public void EndOfRound(int gameID)
        {
            //reset card1 and card2 in Round table for the specific game
            Round round = GetRoundFromGameID(gameID);

            if (GetAmountOfPairs(gameID) == 9)
            {
                EndGame(gameID);
            } else
            {
                CreateAndInsertNewRound(gameID);
            }
        }

        public void CreateAndInsertNewRound(int gameID)
        {
            //create new round and insert it into the Round table
            SqlConnection sqlConnection2 = CreateSQLConnection();

            using SqlCommand cmd2 = new SqlCommand(
             @"INSERT INTO Round (GameID, IndexCard1, IndexCard2, WasItAMatch) 
                 VALUES (@gameID, NULL, NULL, 0)", sqlConnection2);

            //sets the parameters for the SQL command
            cmd2.Parameters.AddWithValue("@gameID", gameID);

            ExecuteNonQuery(sqlConnection2, cmd2);
        }

        public void InsertCard1IntoRound(int gameID, int index)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"UPDATE Round
            SET IndexCard1 = @indexCard1
            WHERE RoundID = (
            SELECT TOP 1 RoundID
            FROM Round
            WHERE GameID = @gameID
            ORDER BY RoundID DESC
            )", sqlConnection);

            cmd.Parameters.AddWithValue("@gameID", gameID);
            cmd.Parameters.AddWithValue("@indexCard1", index);

            ExecuteNonQuery (sqlConnection, cmd);
        }

        public void InsertCard2IntoRound(int gameID, int index)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"UPDATE Round
            SET IndexCard2 = @indexCard2
            WHERE RoundID = (
            SELECT TOP 1 RoundID
            FROM Round
            WHERE GameID = @gameID
            ORDER BY RoundID DESC
            )", sqlConnection);

            cmd.Parameters.AddWithValue("@gameID", gameID);
            cmd.Parameters.AddWithValue("@indexCard2", index);

            ExecuteNonQuery(sqlConnection, cmd);
        }

        public void EndGame(int gameID)
        {
            //set GameState to Finished in the Game table
            SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
             @"UPDATE Game
             SET State = @state
             WHERE GameID = @gameID", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@state", GameState.Finished);
            cmd.Parameters.AddWithValue("@gameID", gameID);

            ExecuteNonQuery(sqlConnection, cmd);

            //determine winner and set Winner attribute in the Game table
            //the player with the most matched cards wins
            using SqlCommand cmd2 = new SqlCommand(
             @"UPDATE Game
             SET Winner = 
             (SELECT TOP 1 PlayerMatchedTo 
              FROM GameCard 
              WHERE GameID = @gameID 
              GROUP BY PlayerMatchedTo 
              ORDER BY COUNT(*) DESC)
             WHERE GameID = @gameID", sqlConnection);

            cmd2.Parameters.AddWithValue("@gameID", gameID);
            ExecuteNonQuery(sqlConnection, cmd2);


        }


        /// <summary>
        /// Retrieves the winner of the specified game by its gameID
        /// </summary>
        /// <param name="gameID">The ID of the current game</param>
        /// <returns>The UserID of the winner</returns>
        public int GetWinner(int gameID)
        {
            int winnerID = 0;

            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
             @"SELECT Winner FROM Game 
             WHERE GameID = @gameID", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@gameID", gameID);
            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);

            if (!reader.Read())
                return winnerID;

            winnerID = (int)reader["Winner"];

            return winnerID;
        }

        public User GetUserByID(int userID) //Funkar inte, sätter userID till 0
        {
            User user = new User();
            using SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Users WHERE UserID = @userID", sqlConnection);

            cmd.Parameters.AddWithValue("@userID", userID);

            sqlConnection.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())  // Viktigt! Kontrollera att det finns en rad.
            {
                user = new User();
                user.UserID = (int)reader["UserID"];
                user.UserName = reader["UserName"].ToString();
            }
			
            return user;

        }

        /// <summary>
        /// Creates a SQL connection to the database
        /// </summary>
        /// <returns>A SQL Connection to the database</returns>
        private SqlConnection CreateSQLConnection ()
        {
            //Connection string to the database
            var cs = "Server=tcp:memorydatabasteknik.database.windows.net,1433;" +
             "Initial Catalog=Memory_databasteknik;" +
            "User ID=memorydatabas;" +
            "Password=1Vigillarintedetta;" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=30;";
            return new SqlConnection(cs);
        }

        private SqlDataReader ExecuteReader(SqlConnection sqlConnection, SqlCommand sqlCommand)
        {
            sqlConnection.Open();
            return sqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        private int ExecuteNonQuery(SqlConnection sqlConnection, SqlCommand sqlCommand)
        {
            sqlConnection.Open();
            int rowsAffected = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
            return rowsAffected;
        }   
    }
}
