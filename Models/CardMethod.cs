namespace MEMORY.Models
{
    /// <summary>
    /// Class which purpose is to provide methods for CardDetail operations.
    /// Only manipulates a card at a time.
    /// </summary>
    public class CardMethod
    {
        public void Flip(CardDetail cardDetail)
        {
            if (!cardDetail.IsMatched)
            {
                cardDetail.IsFlipped = true;
            }
        }
        public void Hide(CardDetail cardDetail)
        {
            if (!cardDetail.IsMatched)
            {
                cardDetail.IsFlipped = false;
            }
        }
    }
}
