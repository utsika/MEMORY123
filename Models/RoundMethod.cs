namespace MEMORY.Models
{
    //ska vi ha en konstruktor som hämtar korten från databasen?????
    public class RoundMethod
    {
        //metod för att kolla om det är en match
        //tar in två kortnamn och returnerar true eller false
        //ska metoden bara ta in kortnamnen eller hela RoundDetail objekt?????
        public Boolean IsItAMatch(string cardName1, string cardName2)
        {
            if (cardName1 == cardName2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    //Ska vi skapa konstruktor här?????
}
