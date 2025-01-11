using System;

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
        CurrentPlayer = "White"; // Initialize with a default value
    }

    public void StartGame()
    {
        Chessboard.InitializeBoard();
    }

    public bool MakeMove(string move)
    {   
        // Parsing the move
        string[] parts = move.Split('-');

        string pieceType = parts[0];
        string startPos = parts[1];
        string endPos = parts[2];

        if (MoveValidator.ValidateMove(move, startPos, endPos, Chessboard, CurrentPlayer, pieceType))
        {
            Console.WriteLine($"Valid move: {move}");
            Chessboard.ApplyMove(startPos, endPos, CurrentPlayer);
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
