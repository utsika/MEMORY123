namespace MEMORY.Models
{
    public class RoundMethod
    {
        //metod för att kolla om det är en match
        //tar in två kortnamn och returnerar true eller false
        public bool IsItAMatch(RoundDetail roundDetail)
        {
            return roundDetail.Card1.CardName == roundDetail.Card2.CardName;
        }

    }

}
