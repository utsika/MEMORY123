using Microsoft.Data.SqlClient;
namespace MEMORY.Models
{
    public class User
    {
        //publika egenskaper
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Passwords { get; set; }

        //publika metoder
        public int InsertUser(UserDetail userDetail, out string errorMessage)
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
        
        //hårdkodat TA BORT SENARE!!!
        String sqlstring = "INSERT INTO Users (UserName, Email, Passwords) VALUES ('UserName', 'Email', 'Password')";
        SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
        try
        {
            sqlConnection.Open();
            int i = 0;
            i = sqlCommand.ExecuteNonQuery();
            if (i == 1) { errorMessage = ""; }
            else { errorMessage = "Insertion failed."; }
            return i;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return 0;
        }
            //stänger anslutningen
            finally
            {
            sqlConnection.Close();
        }
    }
        //DELETE USER METHOD TO DO LATER
        //konstruktor
        public User()
        { }
    }
}
