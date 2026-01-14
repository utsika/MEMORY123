using Microsoft.Data.SqlClient;
namespace MEMORY.Models
{
    public class User
    {
        //publika egenskaper
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Passwords { get; set; }

        //publika metoder
        
        //DELETE USER METHOD TO DO LATER
        //konstruktor
        public User()
        { }
    }
}
