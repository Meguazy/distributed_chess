using Orleans;
using System.Threading.Tasks;

public class PlayerGrain : Grain, IPlayerGrain
{
    private string _name = string.Empty;

    public Task<string> GetNameAsync() => Task.FromResult(_name);

    public Task SetNameAsync(string name)
    {
        _name = name;
        return Task.CompletedTask;
    }
}
