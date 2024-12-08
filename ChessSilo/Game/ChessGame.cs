public class ChessGame
{
    public Player PlayerWhite { get; set; }
    public Player PlayerBlack { get; set; }
    public string CurrentPlayer { get; private set; }
    public Chessboard Chessboard { get; set; }
    
    public ChessGame(Player playerWhite, Player playerBlack, Chessboard chessboard)
    {
        PlayerWhite = playerWhite;
        PlayerBlack = playerBlack;
        Chessboard = chessboard;
    }

    public void StartGame()
    {
        CurrentPlayer = "White"; // White player starts
        Chessboard.InitializeBoard();
    }

    public bool MakeMove(string move)
    {
        if (MoveValidator.ValidateMove(move, Chessboard, CurrentPlayer))
        {
            Chessboard.ApplyMove(move, CurrentPlayer);
            CurrentPlayer = CurrentPlayer == "White" ? "Black" : "White";
            return true;
        }
        return false;
    }

    public class Player
    {
        public string Name { get; }

        public Player(string name)
        {
            Name = name;
        }
    }
}
