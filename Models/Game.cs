using Microsoft.Data.SqlClient;

namespace MEMORY.Models

{
    public enum GameState
    {
        InProgress,
        Finished,
        Pending
    }
    public class Game
    {

        //publika egenskaper
        public int GameID { get; set; }
        public DateTime CreatedWhen { get; set; }
        public int CurrentPlayer { get; set; }
        public int Player1 { get; set; }
        public int? Player2 { get; set; }
        public int AmountOfPairs { get; set; }
        public int? Winner { get; set; }
        public string RoomCode { get; set; }
        public GameState State { get; set; }


        //metoder

       

        public Game()
        {  }
    }

   
}
