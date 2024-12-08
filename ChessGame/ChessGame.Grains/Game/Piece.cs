// ChessGame.Grains/Game/Piece.cs
namespace ChessGame.Grains.Game
{
    public class Piece
    {
        public string Type { get; set; }  // Example: "King", "Queen", "Pawn"
        public string Color { get; set; } // Example: "White", "Black"

        public Piece(string type, string color)
        {
            Type = type;
            Color = color;
        }

        public bool IsValidMove(string start, string end)
        {
            // Implement logic to check if a piece's move is valid according to chess rules
            return true;
        }
    }
}
