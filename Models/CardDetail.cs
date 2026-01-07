namespace MEMORY.Models
{
    public class CardDetail
    {
        //publika egenskaper
        public int CardID { get; set; }
        public string CardName { get; set; }
        public int Index { get; set; } //1-16

        public Boolean IsMatched { get; set; }
        public Boolean IsFlipped { get; set; }

        //GameID foreign key hur?????????????

        //konstruktor
        public CardDetail()
        {
            
        }
    }
}
