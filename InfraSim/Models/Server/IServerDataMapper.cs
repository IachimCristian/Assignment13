using System;
using System.Collections.Generic;

namespace InfraSim.Models.Server
{
    public interface IServerDataMapper
    {
        List<IServer> GetAll();
        IServer? Get(Guid id);
        void Insert(IServer server);
        void Remove(IServer server);
    }
} 