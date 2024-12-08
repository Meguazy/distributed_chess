// ChessGame.Grains/Interfaces/IGameGrain.cs
using Orleans;

namespace ChessGame.Grains.Interfaces
{
    public interface IGameGrain : IGrainWithGuidKey
    {
        Task StartGame(string player1, string player2);
        Task<string> SubmitMove(string playerId, string move);
        Task<string[,]> GetBoardState();
    }
}
