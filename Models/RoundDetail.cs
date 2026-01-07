namespace MEMORY.Models
{
    public class RoundDetail
    {
        //publika egenskaper
        public int RoundID { get; set; }

        public CardDetail Card1 { get; set; }
        public CardDetail Card2 { get; set; }
        //inte ha dessa!!!!!!!!!!
        //public int CardID1 { get; set; }
        //public int CardID2 { get; set; }

        public Boolean WasItAMatch { get; set; }
        
        //GameID, UserID, CardID1, CardID2 foreign keys hur?????????????

        //konstruktor
        public RoundDetail()
        { }
    }
}
