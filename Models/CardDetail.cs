namespace MEMORY.Models
{
    public class CardDetail
    {
        //publika egenskaper
        public int CardID { get; set; }
        public string CardName { get; set; }
        public int X { get; set; }

        public int Y { get; set; }
        public Boolean IsMatched { get; set; }

        //GameID foreign key hur?????????????

        //konstruktor
        public CardDetail()
        { }
    }
}
