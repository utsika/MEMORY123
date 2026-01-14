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

        //public bool IsItAMatch()
        //{
        //    if (CardID1 == null || CardID2 == null)
        //        return false;

        //    bool match = CardID1.CardName == CardID2.CardName;

        //    if (match)
        //    {
        //        //locks cards if a match
        //        CardID1.IsMatched = true;
        //        CardID2.IsMatched = true;
        //        CardID1.IsFlipped = true;
        //        CardID1.IsFlipped = true;
                
        //        //AmountOfPairs ++
        //    }
        //    else
        //    {
        //        //hides cards again if not a match
        //        CardID1.IsFlipped = false;
        //        CardID2.IsFlipped = false;
        //        //Kan ha detta i Controllern istället?!
        //        //SwitchPlayer();
        //    }
        //    return match;
        //}       

        //public Card GetCard1()
        //{
        //    return card1;
        //}

        //public Card GetCard2()
        //{
        //    return card2;
        //}

        //When done with the round, reset the stored cards
        //public void ResetRound()
        //{
        //    CardID1 = null;
        //    CardID2 = null;
        //}

        //metod för att kolla om det är en match
        //tar in två kortnamn och returnerar true eller false
        //public bool IsItAMatch(RoundDetail roundDetail)
        //{
        //    return roundDetail.Card1.CardName == roundDetail.Card2.CardName;
        //    //om IsItAMatch är true så är det en match -> uppdatera kortens IsMatched till true
        //}

        public Round()
        { }
    }

}
