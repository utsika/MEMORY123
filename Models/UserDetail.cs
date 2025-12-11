namespace MEMORY.Models
{
    public class UserDetail
    {
        //publika egenskaper
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        //konstruktor
        public UserDetail()
        {}
    }
}
