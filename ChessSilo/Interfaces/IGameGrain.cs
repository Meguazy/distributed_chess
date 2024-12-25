using Orleans;
using System.Threading.Tasks;

public interface IGameGrain : IGrainWithGuidKey
{
    Task<bool> StartGameAsync(IPlayerGrain playerWhite, IPlayerGrain playerBlack);
    Task<bool> MakeMoveAsync(string move);
    Task<string[,]> GetBoardStateAsync();

}
