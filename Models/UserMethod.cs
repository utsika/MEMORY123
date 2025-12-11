using Microsoft.Data.SqlClient;
namespace MEMORY.Models
{
    public class UserMethod
    {
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
        //hårdkodat
        String sqlstring = "INSERT INTO Users (UserName, Email, Passwords) VALUES ('UserName', 'Email', 'Passwords')";
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
    
        //konstruktor
        public UserMethod()
        { }
    }
}
