namespace MEMORY.Models
{
    public class RoundDetail
    {
        //publika egenskaper
        public int RoundID { get; set; }
        public Boolean WasItAMatch { get; set; }
        
        //GameID, UserID, CardID1, CardID2 foreign keys hur?????????????

        //konstruktor
        public RoundDetail()
        { }
    }
}
