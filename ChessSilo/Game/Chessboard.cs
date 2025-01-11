using System;
using System.Collections.Generic;

public class Chessboard
{
    public Dictionary<string, Piece?> Board { get; set; }

    public Chessboard()
    {
        Board = new Dictionary<string, Piece?>();
        InitializeBoard();
    }

    public void InitializeBoard()
    {
        // Initialize the chessboard with empty squares
        for (int row = 1; row <= 8; row++)
        {
            for (char col = 'a'; col <= 'h'; col++)
            {
                string position = $"{col}{row}";
                Board[position] = null; // Empty square
            }
        }

        // 🟢 Place Pawns
        for (char col = 'a'; col <= 'h'; col++)
        {
            Board[$"{col}2"] = new Pawn("White"); // White pawns on row 2
            Board[$"{col}7"] = new Pawn("Black"); // Black pawns on row 7
        }

        // 🟢 Place Rooks
        Board["a1"] = new Rook("White");
        Board["h1"] = new Rook("White");
        Board["a8"] = new Rook("Black");
        Board["h8"] = new Rook("Black");

        // 🟢 Place Knights
        Board["b1"] = new Knight("White");
        Board["g1"] = new Knight("White");
        Board["b8"] = new Knight("Black");
        Board["g8"] = new Knight("Black");

        // 🟢 Place Bishops
        Board["c1"] = new Bishop("White");
        Board["f1"] = new Bishop("White");
        Board["c8"] = new Bishop("Black");
        Board["f8"] = new Bishop("Black");

        // 🟢 Place Queens
        Board["d1"] = new Queen("White");
        Board["d8"] = new Queen("Black");

        // 🟢 Place Kings
        Board["e1"] = new King("White");
        Board["e8"] = new King("Black");
    }

    public string[,] GetBoardState()
    {
        string[,] boardState = new string[8, 8];

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                // Convert array indices to chess notation
                char file = (char)('a' + col); // 'a' corresponds to column 0, 'b' to 1, etc.
                int rank = 8 - row;            // Rank is 8 at row 0, 7 at row 1, etc.
                string position = $"{file}{rank}";

                if (Board[position] != null)
                {
                    boardState[row, col] = Board[position].Type[0].ToString() + Board[position].Color[0].ToString();
                }
                else
                {
                    boardState[row, col] = "--";
                }
            }
        }

        return boardState;
    }

    public void ApplyMove(string startPos, string endPos, string pieceType)
    {
        // Log the parsed move
        Console.WriteLine($"Piece: {pieceType}, Start: {startPos}, End: {endPos}");

        // Perform the move on the chessboard
        if (Board[startPos] != null)
        {
            Console.WriteLine($"Piece in start position: {Board[startPos].Type} at {startPos}");
        }
        else
        {
            Console.WriteLine($"No piece at start position: {startPos}");
        }
        if (Board[endPos] != null)
        {
            Console.WriteLine($"Piece in end position: {Board[endPos].Type} at {endPos}");
        }
        else
        {
            Console.WriteLine($"No piece at end position: {endPos}");
        }

        Board[endPos] = Board[startPos]; // Move the piece to the end position
        Board[startPos] = null;          // Remove the piece from the start position
        
        if (Board[startPos] != null)
        {
            Console.WriteLine($"Piece in start position: {Board[startPos].Type} at {startPos}");
        }
        else
        {
            Console.WriteLine($"No piece at start position: {startPos}");
        }
        if (Board[endPos] != null)
        {
            Console.WriteLine($"Piece in end position: {Board[endPos].Type} at {endPos}");
        }
        else
        {
            Console.WriteLine($"No piece at end position: {endPos}");
        }
    }
}
