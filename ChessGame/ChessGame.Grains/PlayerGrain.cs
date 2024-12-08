// ChessGame.Grains/PlayerGrain.cs
using ChessGame.Grains.Interfaces;

namespace ChessGame.Grains
{
    public class PlayerGrain : Grain, IPlayerGrain
    {
        private Guid _currentGame;

        public Task JoinGame(Guid gameId)
        {
            _currentGame = gameId;
            return Task.CompletedTask;
        }

        public Task NotifyMove(string move)
        {
            Console.WriteLine($"Player received move: {move}");
            // Here you can broadcast the move to other players or update the UI
            return Task.CompletedTask;
        }
    }
}
