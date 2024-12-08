// ChessGame.Grains/GameGrain.cs
using ChessGame.Grains.Interfaces;

namespace ChessGame.Grains
{
    public class GameGrain : Grain, IGameGrain
    {
        private string[,] _board;
        private string _currentTurn;
        private string _player1;
        private string _player2;

        public override Task OnActivateAsync()
        {
            _board = new string[8, 8]; // Initialize chessboard
            // Set up initial chessboard state, e.g., pieces in their starting positions
            return base.OnActivateAsync();
        }

        public Task StartGame(string player1, string player2)
        {
            _player1 = player1;
            _player2 = player2;
            _currentTurn = player1; // Player 1 starts the game
            return Task.CompletedTask;
        }

        public Task<string> SubmitMove(string playerId, string move)
        {
            if (_currentTurn != playerId)
                return Task.FromResult("Not your turn!");

            // Validate and process the move
            // Update the board accordingly and switch turns
            _currentTurn = _currentTurn == _player1 ? _player2 : _player1;

            return Task.FromResult("Move accepted");
        }

        public Task<string[,]> GetBoardState() => Task.FromResult(_board);
    }
}
