using Microsoft.Data.SqlClient;

namespace MEMORY.Models
{
    public class UserDatabaseMethods
    {
        public User GetUserByUsernameAndPassword(string userName, string password)
        {
            SqlConnection sqlConnection = CreateSQLConnection();

            using SqlCommand cmd = new SqlCommand(
                @"
                SELECT UserID, UserName
                FROM Users
                WHERE UserName = @userName AND Passwords = @password", sqlConnection);

            cmd.Parameters.AddWithValue("@username", userName);
            cmd.Parameters.AddWithValue("@password", password);

            using SqlDataReader reader = ExecuteReader(sqlConnection, cmd);

            if (reader.Read())
            {
                return new User
                {
                    UserID = (int)reader["UserID"],
                    UserName = reader["UserName"].ToString()
                };
            }

            return null;
        }

        public void InsertUser(User user)
        {
            SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Users (UserName, Passwords)
                VALUES (@userName, @password)", sqlConnection);

            cmd.Parameters.AddWithValue("@userName", user.UserName);
            cmd.Parameters.AddWithValue("@password", user.Passwords);

            ExecuteNonQuery(sqlConnection, cmd);
        }

        public void DeleteUser(int userID)
        {
            SqlConnection sqlConnection = CreateSQLConnection();
            using SqlCommand cmd = new SqlCommand(
                @"DELETE FROM Users 
                WHERE UserID = (@userID)", sqlConnection);

            cmd.Parameters.AddWithValue("@userID", userID);

            ExecuteNonQuery(sqlConnection, cmd);
        }

        private SqlConnection CreateSQLConnection()
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
