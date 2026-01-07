namespace MEMORY.Models
{
    /// <summary>
    /// Class which purpose is to provide methods for CardDetail operations.
    /// Only manipulates a card at a time.
    /// </summary>
    public class CardMethod
    {
        //Flips a card
        public void Flip(CardDetail cardDetail)
        {
            if (!cardDetail.IsMatched)
            {
                cardDetail.IsFlipped = !cardDetail.IsFlipped;
            }
        }

        //If the card was not a match, hide it
        //public void Hide(CardDetail cardDetail)
        //{
        //    if (!cardDetail.IsMatched)
        //    {
        //        cardDetail.IsFlipped = false;
        //    }
        //}
        //ANVÄNDS INTE, SKREV ISTÄLLET I ROUNDMETHOD!!!!!!

        //If the cards were a match, lock them face up
        //public void Lock(CardDetail cardDetail)
        //{
        //    if (cardDetail.IsMatched)
        //    {
        //        cardDetail.IsFlipped = true;
        //    }
        //}
        //ANCÄNDS INTE, SKREV ISTÄLLET I ROUNDMETHOD!!!!!!
    }
}
