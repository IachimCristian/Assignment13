using InfraSim.Models.State;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public interface IServerBuilder
    {
        IServerBuilder WithType(ServerType type);
        IServerBuilder WithCapability(IServerCapability capability);
        IServerBuilder WithState(IServerState state);
        Server Build();
    }
} 