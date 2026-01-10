using Microsoft.Data.SqlClient;
using System;
using System.Reflection;

namespace MEMORY.Models
{
    public class DatabaseMethods
    {

        //behövs denna?
        public Card GetCardByIndex(int gameID, int index)
        {
            //Connection string to the database
            var cs = "Server=tcp:memorydatabasteknik.database.windows.net,1433;" +
             "Initial Catalog=Memory_databasteknik;" +
            "User ID=memorydatabas;" +
            "Password=1Vigillarintedetta;" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=30;";
            using var sqlConnection = new SqlConnection(cs);

            //fixa i databas så allt stämmer överens!!!!
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

        public void InsertGame(Game game)
        {
            using var sqlConnection = CreateSQLConnection();

            //fixa i databas så allt stämmer överens!!!!
            //lägg in alla attribut till Game!!!
            using SqlCommand cmd = new SqlCommand(
             @"INSERT INTO Game VALUES ()", sqlConnection);

            //sets the parameters for the SQL command
            //detta gäller för alla attribut i Game klassen
            cmd.Parameters.AddWithValue("@gameID", game.GameID);
            cmd.Parameters.AddWithValue("@createdWhen", game.CreatedWhen);
            cmd.Parameters.AddWithValue("@player1", game.Player1);
            cmd.Parameters.AddWithValue("@player2", game.Player2);
            cmd.Parameters.AddWithValue("@currentPlayer", game.CurrentPlayer);
            cmd.Parameters.AddWithValue("@roomCode", game.RoomCode);
            cmd.Parameters.AddWithValue("@state", game.State);
            cmd.Parameters.AddWithValue("@amountOfPairs", game.AmountOfPairs);
            cmd.Parameters.AddWithValue("@winner", game.Winner);

            ExecuteSQLConnection(sqlConnection, cmd);
        }

        public void InsertCardList(List<Card> cards, int gameID)
        {
            
            //fixa i databas så allt stämmer överens!!!!
            foreach (var card in cards)
            {                
                using var sqlConnection = CreateSQLConnection();

                using SqlCommand cmd = new SqlCommand(
                 @"INSERT INTO GameCard VALUES ())", sqlConnection);

                cmd.Parameters.AddWithValue("@gameID", gameID);
                cmd.Parameters.AddWithValue("@cardName", card.CardName);
                cmd.Parameters.AddWithValue("@index", card.Index);
                cmd.Parameters.AddWithValue("@isMatched", card.IsMatched);

                ExecuteSQLConnection(sqlConnection, cmd);

            }
        }

        public Game GetGameFromRoomCode(string roomCode)
        {
            Game game = new Game();
            SqlConnection sqlConnection = CreateSQLConnection();

            //fixa i databas så allt stämmer överens!!!!
            using SqlCommand cmd = new SqlCommand(
             @"SELECT GameID FROM Game 
             WHERE RoomCode = @roomCode", sqlConnection);

            //sets the parameters for the SQL command
            cmd.Parameters.AddWithValue("@roomCode", roomCode);
                    
            using SqlDataReader reader = ExecuteSQLConnection(sqlConnection, cmd);
            if (!reader.Read())
                return game;

            //fylla game objektet med data från databasen
            game.GameID = (int)reader["GameID"];
            game.CreatedWhen = (DateTime)reader["CreatedWhen"];
            game.Player1 = (int)reader["Player1"];
            game.Player2 = (int)reader["Player2"];
            game.CurrentPlayer = (int)reader["CurrentPlayer"];
            game.RoomCode = reader["RoomCode"].ToString();
            game.State = (GameState)reader["State"];
            game.AmountOfPairs = (int)reader["AmountOfPairs"];
            game.Winner = reader["Winner"] as int?;


            //returns the retrieved carddetails
            return game;
        }

        //gets/retrieves the cards from a specific game
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

            using SqlDataReader reader = ExecuteSQLConnection(sqlConnection, cmd);
            if (!reader.Read())
                return cards;

            while (reader.Read())
            {
                Card card = new Card();
                card.CardID = (int)reader["CardID"];
                card.CardName = reader["CardName"].ToString();
                card.Index = (int)reader["Index"];
                card.IsMatched = (bool)reader["IsMatched"];
                card.IsFlipped = (bool)reader["IsFlipped"];

                cards.Add(card);
            }
           
            //returns the retrieved carddetails
            return cards;
        }

        //SELECTCARD 
        //FIXA!!!
        //INTE GLÖMMA
        //MYS
        //VERKLIGEN INTE GLÖMMA
        
        //created a connection to database
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

        //executes the sql command
        private SqlDataReader ExecuteSQLConnection(SqlConnection sqlConnection, SqlCommand sqlCommand)
        {
            string errorMessage = "";

            SqlDataReader reader = null;


            try
            {
                sqlConnection.Open();
                int i = 0;
                i = sqlCommand.ExecuteNonQuery();
                if (i != 1) { errorMessage = "Command failed."; }
                else { reader = sqlCommand.ExecuteReader(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(errorMessage);
            }

            //stänger anslutningen
            finally
            {
                sqlConnection.Close();
            }
            return reader;
        }
    }
}
