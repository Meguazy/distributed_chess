using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public static class MoveValidator
{
    // Main method to validate a move
    public static bool ValidateMove(string move, string startPos, string endPos, Chessboard chessboard, string playerColor, string pieceType)
    {
        Dictionary<string, Piece?> board = chessboard.Board;
        // Basic validation to ensure the move format is correct (simplified)
        if (!IsValidMoveFormat(move))
        {
            Console.WriteLine($"Invalid move format: {move}");
            return false;
        }

        // Validate if the start position contains the correct piece
        if (!board.ContainsKey(startPos) || board[startPos]! == null)
        {
            Console.WriteLine($"No piece found at {startPos}.");
            return false;
        }

        if(board.ContainsKey(endPos) && board[endPos]! != null && board[endPos]!.Color == playerColor)
        {
            Console.WriteLine($"Cannot capture your own piece at {endPos}.");
            return false;
        }

        if (board[startPos]!.Type[0].ToString() != pieceType || board[startPos]!.Color != playerColor)
        {
            Console.WriteLine($"The piece at {startPos} is not a {playerColor}'s {board[startPos]!.Type[0]}.");
            return false;
        }

        return true;
    }

    // Validate the basic move format (e.g., "P-e2-e4")
    private static bool IsValidMoveFormat(string move)
    {
        // Example: Regex to check the move format (e.g., "P-e2-e4" or "K-e1-e2")
        var regex = new Regex(@"^[PRNBQK]-[a-h][1-8]-[a-h][1-8]$");
        return regex.IsMatch(move);
    }
}
