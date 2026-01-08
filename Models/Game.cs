using Microsoft.Data.SqlClient;

namespace MEMORY.Models

{
    public enum GameState
    {
        InProgress,
        Finished
    }
    public class Game
    {
        //publika egenskaper
        public int GameID { get; set; }
        public DateTime CreatedWhen { get; set; }

        public int CurrentPlayer { get; set; }

        //metoder

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

        private void SwitchPlayer(Game game)
        {
            game.CurrentPlayer = game.CurrentPlayer == 1 ? 2 : 1;
        }

        public Game()
        {  }
    }

   
}
