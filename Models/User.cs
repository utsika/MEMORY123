using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
namespace MEMORY.Models
{
    public class User
    {
        //publika egenskaper
        public int ID { get; set; }

        [Required(ErrorMessage = "Du måste skriva in ett användarnamn")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Du måste skriva in ett lösenord")]
        [MinLength(6, ErrorMessage = "Lösenordet måste vara minst 6 tecken")]
        [MaxLength(18, ErrorMessage = "Lösenordet får inte vara mer än 18 tecken")]
        public string Passwords { get; set; }
    }
}
