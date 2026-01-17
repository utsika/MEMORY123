namespace MEMORY.Models
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public List<Card> Cards { get; set; }

        // Lägg till namn på spelarna
        public string WinnerName { get; set; }
    }
}


