namespace MEMORY.Models
{
    public class Round
    {

        //publika egenskaper
        public int RoundID { get; set; }

        //public Card? card1 { get; set; }
        //public Card? card2 { get; set; }

        public int UserID { get; set; }
        public int GameID { get; set; }


        //inte ha dessa!!!!!!!!!!
        public int? IndexCard1 { get; set; }
        public int? IndexCard2 { get; set; }

        public Boolean WasItAMatch { get; set; }

        //GameID, UserID, CardID1, CardID2 foreign keys hur?????????????
        

        /*
         * IsFirstCard() == true -> Card1
         * IsFirstCard() == false -> Card2
         */
        public bool IsFirstCard()
        {
            // Om Card1 inte är satt ännu,
            // då är detta första kortet
            return IndexCard1 == null;
        }      
        
        public void SetCard1(int card)
        {
            IndexCard1 = card;
        }

        public void SetCard2(int card)
        {
            IndexCard2 = card;
        }
      
        public Round()
        { }
    }

}
