using Orleans;
using System.Threading.Tasks;

public interface IPlayerGrain : IGrainWithGuidKey
{
    Task<string> GetNameAsync();
    Task SetNameAsync(string name);
}
