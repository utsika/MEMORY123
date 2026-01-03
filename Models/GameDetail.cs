namespace MEMORY.Models
{
    public class GameDetail
    {
        //publika egenskaper
        public int GameID { get; set; }
        public DateTime  CreatedWhen { get; set; }
        public Boolean State {  get; set; }
                
        //konstruktor
        public GameDetail()
        { }
    }
}
