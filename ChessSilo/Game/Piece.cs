using System;

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

// Pawn
public class Pawn : Piece
{
    public Pawn(string color) : base(color, "Pawn") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Example pawn move validation
        // "move" could be in the format "e2-e4"
        // Implement proper pawn movement validation
        Console.WriteLine($"{Color} Pawn trying move: {move}");
        return true; 
    }
}

// Rook
public class Rook : Piece
{
    public Rook(string color) : base(color, "Rook") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Rooks move in straight lines
        Console.WriteLine($"{Color} Rook trying move: {move}");
        return true; 
    }
}

// Knight
public class Knight : Piece
{
    public Knight(string color) : base(color, "Knight") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Knights move in an 'L' shape
        Console.WriteLine($"{Color} Knight trying move: {move}");
        return true; 
    }
}

// Bishop
public class Bishop : Piece
{
    public Bishop(string color) : base(color, "Bishop") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Bishops move diagonally
        Console.WriteLine($"{Color} Bishop trying move: {move}");
        return true; 
    }
}

// Queen
public class Queen : Piece
{
    public Queen(string color) : base(color, "Queen") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Queens move in any direction
        Console.WriteLine($"{Color} Queen trying move: {move}");
        return true; 
    }
}

// King
public class King : Piece
{
    public King(string color) : base(color, "King") { }

    public override bool IsValidMove(string move, Chessboard board)
    {
        // Kings move one square in any direction
        Console.WriteLine($"{Color} King trying move: {move}");
        return true; 
    }
}
