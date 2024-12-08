// ChessGame.Grains/Game/ChessGame.cs
using System;

namespace ChessGame.Grains.Game
{
    public class ChessGame
    {
        public string[,] Board { get; private set; }
        public string CurrentTurn { get; private set; }
        public string Player1 { get; private set; }
        public string Player2 { get; private set; }

        public ChessGame(string player1, string player2)
        {
            Player1 = player1;
            Player2 = player2;
            Board = InitializeBoard();
            CurrentTurn = Player1;  // Player1 starts the game
        }

        private string[,] InitializeBoard()
        {
            // Initialize the chessboard with pieces
            var board = new string[8, 8];
            // Place pieces on the board, e.g., "P" for pawn, "K" for king, etc.
            // Here, use simplified representation for example purposes.
            // You should replace it with proper chess notation.
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = "P"; // Player 1's pawns
                board[6, i] = "p"; // Player 2's pawns
            }
            // Setup other pieces similarly
            return board;
        }

        public bool MakeMove(string move)
        {
            // Validate and execute the move, alternating turns
            // You should implement move validation here
            // Switch turns after a valid move
            CurrentTurn = (CurrentTurn == Player1) ? Player2 : Player1;
            return true;
        }

        public string GetBoardState()
        {
            var boardState = "";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardState += Board[i, j] ?? ".";
                }
                boardState += "\n";
            }
            return boardState;
        }
    }
}
