public abstract class Piece
{
    public string Color { get; }
    public string Type { get; }

    protected Piece(string color, string type)
    {
        Color = color;
        Type = type;
    }

    public abstract bool IsValidMove(string move, Chessboard board);
}

public class Pawn : Piece
{
    public Pawn(string color) : base(color, "Pawn") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Implement pawn-specific move validation
        return true;
    }
}

// Additional piece types (Rook, King, etc.) should be added similarly
