using System;

public class Chessboard
{
    private Piece[,] Board { get; set; }

    public Chessboard()
    {
        Board = new Piece[8, 8];
    }

    public void InitializeBoard()
    {
        // Initialize the chessboard        
        // 🟢 Place Pawns
        for (int i = 0; i < 8; i++)
        {
            Board[1, i] = new Pawn("Black"); // Black pawns on row 1
            Board[6, i] = new Pawn("White"); // White pawns on row 6
        }

        // 🟢 Place Rooks
        Board[0, 0] = new Rook("Black");
        Board[0, 7] = new Rook("Black");
        Board[7, 0] = new Rook("White");
        Board[7, 7] = new Rook("White");

        // 🟢 Place Knights
        Board[0, 1] = new Knight("Black");
        Board[0, 6] = new Knight("Black");
        Board[7, 1] = new Knight("White");
        Board[7, 6] = new Knight("White");

        // 🟢 Place Bishops
        Board[0, 2] = new Bishop("Black");
        Board[0, 5] = new Bishop("Black");
        Board[7, 2] = new Bishop("White");
        Board[7, 5] = new Bishop("White");

        // 🟢 Place Queens
        Board[0, 3] = new Queen("Black");
        Board[7, 3] = new Queen("White");

        // 🟢 Place Kings
        Board[0, 4] = new King("Black");
        Board[7, 4] = new King("White");
    }

    public string[,] GetBoardState()
    {
        string[,] boardState = new string[8, 8];

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (Board[row, col] != null)
                {
                    boardState[row, col] = Board[row, col].Type[0].ToString() + Board[row, col].Color[0].ToString();
                }
                else
                {
                    boardState[row, col] = "--";
                }
            }
        }

        return boardState;
    }

    public void ApplyMove(string move, string playerColor)
    {
        // Example move: "P-e2-e3"
        // Parsing the move
        string[] parts = move.Split('-'); // Splits the move into ["P", "e2", "e3"]

        if (parts.Length != 3)
        {
            Console.WriteLine("Invalid move format.");
            return;
        }

        string pieceType = parts[0];   // "P" (Pawn)
        string startPos = parts[1];    // "e2"
        string endPos = parts[2];      // "e3"

        // Extracting the starting and ending positions
        int startFile = startPos[0] - 'a'; // 'e' -> 4 (file)
        int startRank = 8 - (startPos[1] - '0'); // '2' -> 6 (rank, reversed)

        int endFile = endPos[0] - 'a'; // 'e' -> 4 (file)
        int endRank = 8 - (endPos[1] - '0'); // '3' -> 5 (rank, reversed)

        // Log the parsed move
        Console.WriteLine($"Piece: {pieceType}, Start: {startPos} (File: {startFile}, Rank: {startRank}), End: {endPos} (File: {endFile}, Rank: {endRank})");

        // Validate the move for a Pawn (P)
        if (pieceType == "P")
        {
            // Logic to apply Pawn move (very basic implementation, can be expanded)
            if (startFile == endFile && Math.Abs(startRank - endRank) == 1)
            {
                Console.WriteLine($"Pawn moved from {startPos} to {endPos}");

                // Perform the move on the chessboard
                Board[endRank, endFile] = Board[startRank, startFile]; // Move the pawn
                Board[startRank, startFile] = null; // Remove the pawn from the start position

                // Update board logic would go here, like marking the piece's new position on the board.
            }
            else
            {
                Console.WriteLine($"Invalid Pawn move from {startPos} to {endPos}");
            }
        }
        else
        {
            // Handle other piece types (Rook, Knight, Bishop, etc.)
            Console.WriteLine("Only Pawn moves are handled for now.");
        }
    }


    public bool IsCorrectPieceForPlayer(string square, string playerColor)
    {
        // Check if the piece at the given square belongs to the player
        return true;
    }

    public Piece GetPieceAt(string square)
    {
        // Get the piece at the given square
        return new Pawn("Black");
    }

    public bool IsSquareOccupied(string square)
    {
        // Check if the square is occupied by a piece
        return true;
    }

    public bool IsPathClear(string startSquare, string endSquare)
    {
        // Check if the path between start and end squares is clear
        return true;
    }
}
