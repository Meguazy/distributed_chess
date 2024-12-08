// ChessGame.Grains/Interfaces/IPlayerGrain.cs
using Orleans;

namespace ChessGame.Grains.Interfaces
{
    public interface IPlayerGrain : IGrainWithStringKey
    {
        Task JoinGame(Guid gameId);
        Task NotifyMove(string move);
    }
}
