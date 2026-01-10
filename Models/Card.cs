namespace MEMORY.Models
{
    /// <summary>
    /// Class which purpose is to provide methods for CardDetail operations.
    /// Only manipulates a card at a time.
    /// </summary>
    public class Card
    {
        //publika egenskaper
        public int CardID { get; set; }
        public string CardName { get; set; }
        public int Index { get; set; } //1-16

        public Boolean IsMatched { get; set; }
        public Boolean IsFlipped { get; set; }

        public Card(Card card)
        {
            this.CardID = card.CardID;
            this.CardName = card.CardName;
            this.Index = card.Index;
            this.IsMatched = card.IsMatched;
            this.IsFlipped = card.IsFlipped;
        }

        //Flips a card
        public void Flip(Card card)
        {
            if (!card.IsMatched)
            {
                card.IsFlipped = !card.IsFlipped;
            }
        }

        //If the card was not a match, hide it
        public void Hide(Card card)
        {
            if (!card.IsMatched)
            {
                card.IsFlipped = false;
            }
        }
        //ANVÄNDS INTE, SKREV ISTÄLLET I ROUNDMETHOD!!!!!!

        //If the cards were a match, lock them face up
        public void Lock(Card card)
        {
            if (card.IsMatched)
            {
                card.IsFlipped = true;
            }
        }
        //ANCÄNDS INTE, SKREV ISTÄLLET I ROUNDMETHOD!!!!!!

        public Card () 
        { }

    }
}
