namespace MEMORY.Models
{
    public class Round
    {

        //publika egenskaper
        public int RoundID { get; set; }

        public Card? card1 { get; set; }
        public Card? card2 { get; set; }

        //inte ha dessa!!!!!!!!!!
        //public int CardID1 { get; set; }
        //public int CardID2 { get; set; }

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
            return card1 == null;
        }      
        
        public void SetCard1(Card card)
        {
            card1 = card;
        }

        public void SetCard2(Card card)
        {
            card2 = card;
        }

        public bool IsItAMatch()
        {
            if (card1 == null || card2 == null)
                return false;

            bool match = card1.CardName == card2.CardName;

            if (match)
            {
                //locks cards if a match
                card1.IsMatched = true;
                card2.IsMatched = true;
                card1.IsFlipped = true;
                card2.IsFlipped = true;
                
                //AmountOfPairs ++
            }
            else
            {
                //hides cards again if not a match
                card1.IsFlipped = false;
                card2.IsFlipped = false;
                //Kan ha detta i Controllern istället?!
                //SwitchPlayer();
            }
            return match;
        }       

        //public Card GetCard1()
        //{
        //    return card1;
        //}

        //public Card GetCard2()
        //{
        //    return card2;
        //}

        //When done with the round, reset the stored cards
        public void ResetRound()
        {
            card1 = null;
            card2 = null;
        }

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
