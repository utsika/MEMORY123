namespace MEMORY.Models
{
    public enum GameState
    {
        InProgress,
        Finished
    }
    public class GameDetail
    {
        //publika egenskaper
        public int GameID { get; set; }
        public DateTime  CreatedWhen { get; set; }

                
        //konstruktor
        public GameDetail()
        { }
    }
}
