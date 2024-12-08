using System;
using System.Text.RegularExpressions;

public static class MoveValidator
{
    // Main method to validate a move
    public static bool ValidateMove(string move, Chessboard board, string currentPlayer)
    {
        // Basic validation to ensure the move format is correct (simplified)
        if (!IsValidMoveFormat(move))
        {
            Console.WriteLine($"Invalid move format: {move}");
            return false;
        }

        string pieceType = move.Substring(0, 1); // Assume move is like "P-e2-e4"
        switch (pieceType.ToUpper())
        {
            case "P": // Pawn
                return ValidatePawnMove(move, board, currentPlayer);
            case "R": // Rook
                return ValidateRookMove(move, board, currentPlayer);
            case "N": // Knight
                return ValidateKnightMove(move, board, currentPlayer);
            case "B": // Bishop
                return ValidateBishopMove(move, board, currentPlayer);
            case "Q": // Queen
                return ValidateQueenMove(move, board, currentPlayer);
            case "K": // King
                return ValidateKingMove(move, board, currentPlayer);
            default:
                Console.WriteLine($"Unsupported piece: {pieceType}");
                return false;
        }
    }

    // Validate the basic move format (e.g., "P-e2-e4")
    private static bool IsValidMoveFormat(string move)
    {
        // Example: Regex to check the move format (e.g., "P-e2-e4" or "K-e1-e2")
        var regex = new Regex(@"^[PRNBQK]-[a-h][1-8]-[a-h][1-8]$");
        return regex.IsMatch(move);
    }

    // Validate Pawn move (simplified)
    private static bool ValidatePawnMove(string move, Chessboard board, string currentPlayer)
    {
        string startSquare = move.Split('-')[1]; // e.g., "e2"
        string endSquare = move.Split('-')[2];   // e.g., "e4"
        
        // Check if start square contains the correct piece for the current player
        if (!board.IsCorrectPieceForPlayer(startSquare, currentPlayer))
        {
            Console.WriteLine($"Invalid piece for player {currentPlayer} at {startSquare}");
            return false;
        }

        // Implement actual pawn movement logic (one square forward, two squares on first move, etc.)
        // Example simplified validation:
        if (startSquare[0] == endSquare[0] && Math.Abs(int.Parse(startSquare[1].ToString()) - int.Parse(endSquare[1].ToString())) == 1)
        {
            // Check if target square is free or contains an enemy piece (for captures)
            if (board.IsSquareOccupied(endSquare))
            {
                var targetPiece = board.GetPieceAt(endSquare);
                if (targetPiece.Color == currentPlayer) // Cannot land on own piece
                {
                    Console.WriteLine($"Invalid move: Target square {endSquare} occupied by your own piece.");
                    return false;
                }
            }
            return true;
        }

        // Additional pawn movement rules can be added (en passant, promotion, etc.)
        return false;
    }

    // Validate Rook move
    private static bool ValidateRookMove(string move, Chessboard board, string currentPlayer)
    {
        string startSquare = move.Split('-')[1];
        string endSquare = move.Split('-')[2];

        // Check if start square contains the correct piece for the current player
        if (!board.IsCorrectPieceForPlayer(startSquare, currentPlayer))
        {
            Console.WriteLine($"Invalid piece for player {currentPlayer} at {startSquare}");
            return false;
        }

        // Rook moves in straight lines (rows or columns)
        if (startSquare[0] == endSquare[0] || startSquare[1] == endSquare[1])
        {
            // Check if the path is clear (no pieces in between)
            if (board.IsPathClear(startSquare, endSquare))
            {
                // Check if target square is free or contains an enemy piece (for captures)
                if (board.IsSquareOccupied(endSquare))
                {
                    var targetPiece = board.GetPieceAt(endSquare);
                    if (targetPiece.Color == currentPlayer) // Cannot land on own piece
                    {
                        Console.WriteLine($"Invalid move: Target square {endSquare} occupied by your own piece.");
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    // Validate Knight move
    private static bool ValidateKnightMove(string move, Chessboard board, string currentPlayer)
    {
        string startSquare = move.Split('-')[1];
        string endSquare = move.Split('-')[2];

        // Check if start square contains the correct piece for the current player
        if (!board.IsCorrectPieceForPlayer(startSquare, currentPlayer))
        {
            Console.WriteLine($"Invalid piece for player {currentPlayer} at {startSquare}");
            return false;
        }

        // Knight moves in "L" shape (two squares in one direction, one square perpendicular)
        int xDiff = Math.Abs(startSquare[0] - endSquare[0]);
        int yDiff = Math.Abs(startSquare[1] - endSquare[1]);

        if ((xDiff == 2 && yDiff == 1) || (xDiff == 1 && yDiff == 2))
        {
            // Check if target square is free or contains an enemy piece (for captures)
            if (board.IsSquareOccupied(endSquare))
            {
                var targetPiece = board.GetPieceAt(endSquare);
                if (targetPiece.Color == currentPlayer) // Cannot land on own piece
                {
                    Console.WriteLine($"Invalid move: Target square {endSquare} occupied by your own piece.");
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    // Validate Bishop move
    private static bool ValidateBishopMove(string move, Chessboard board, string currentPlayer)
    {
        string startSquare = move.Split('-')[1];
        string endSquare = move.Split('-')[2];

        // Check if start square contains the correct piece for the current player
        if (!board.IsCorrectPieceForPlayer(startSquare, currentPlayer))
        {
            Console.WriteLine($"Invalid piece for player {currentPlayer} at {startSquare}");
            return false;
        }

        // Bishop moves diagonally, so the absolute difference in ranks and files should be equal
        if (Math.Abs(startSquare[0] - endSquare[0]) == Math.Abs(startSquare[1] - endSquare[1]))
        {
            // Check if the path is clear (no pieces in between)
            if (board.IsPathClear(startSquare, endSquare))
            {
                // Check if target square is free or contains an enemy piece (for captures)
                if (board.IsSquareOccupied(endSquare))
                {
                    var targetPiece = board.GetPieceAt(endSquare);
                    if (targetPiece.Color == currentPlayer) // Cannot land on own piece
                    {
                        Console.WriteLine($"Invalid move: Target square {endSquare} occupied by your own piece.");
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    // Validate Queen move (combination of Rook and Bishop moves)
    private static bool ValidateQueenMove(string move, Chessboard board, string currentPlayer)
    {
        string startSquare = move.Split('-')[1];
        string endSquare = move.Split('-')[2];

        // Check if start square contains the correct piece for the current player
        if (!board.IsCorrectPieceForPlayer(startSquare, currentPlayer))
        {
            Console.WriteLine($"Invalid piece for player {currentPlayer} at {startSquare}");
            return false;
        }

        // Queen moves like both a rook and bishop (straight or diagonal)
        if (ValidateRookMove(move, board, currentPlayer) || ValidateBishopMove(move, board, currentPlayer))
        {
            return true;
        }
        return false;
    }

    // Validate King move
    private static bool ValidateKingMove(string move, Chessboard board, string currentPlayer)
    {
        string startSquare = move.Split('-')[1];
        string endSquare = move.Split('-')[2];

        // Check if start square contains the correct piece for the current player
        if (!board.IsCorrectPieceForPlayer(startSquare, currentPlayer))
        {
            Console.WriteLine($"Invalid piece for player {currentPlayer} at {startSquare}");
            return false;
        }

        // King moves one square in any direction
        int xDiff = Math.Abs(startSquare[0] - endSquare[0]);
        int yDiff = Math.Abs(startSquare[1] - endSquare[1]);

        if ((xDiff <= 1 && yDiff <= 1))
        {
            // Check if target square is free or contains an enemy piece (for captures)
            if (board.IsSquareOccupied(endSquare))
            {
                var targetPiece = board.GetPieceAt(endSquare);
                if (targetPiece.Color == currentPlayer) // Cannot land on own piece
                {
                    Console.WriteLine($"Invalid move: Target square {endSquare} occupied by your own piece.");
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}
