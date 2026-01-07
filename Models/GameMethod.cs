using Microsoft.Data.SqlClient;

namespace MEMORY.Models

{
    public class GameMethod
    {
       
    public CardDetail GetCardByIndex(int gameID, int index)
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
            return new CardDetail
            {
                CardID = (int)reader["CardID"],
                CardName = reader["CardName"].ToString()
            };
        }



        private void SwitchPlayer(GameDetail gameDetail)
        {
            gameDetail.CurrentPlayer = gameDetail.CurrentPlayer == 1 ? 2 : 1;
        }
    }

   
}
