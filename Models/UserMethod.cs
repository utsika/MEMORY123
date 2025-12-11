using Microsoft.Data.SqlClient;
using MEMORY.Models;
   
    public class UserMethod
    {
        //publika metoder
        public int InsertUser(UserDetail userDetail, out string errorMessage)
        {
            //Connection string to the database
            SqlConnection sqlConnection = new SqlConnection("Server=tcp:memorydatabasteknik.database.windows.net,1433;Initial Catalog=Memory_databasteknik;Persist Security Info=False;User ID=memorydatabas;Password={1Vigillarintedetta};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            //hårdkodad sträng
            String sqlstring = "INSERT INTO User (UserName, Email, Password) VALUES ('UserName', 'Email', 'Password')";
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
