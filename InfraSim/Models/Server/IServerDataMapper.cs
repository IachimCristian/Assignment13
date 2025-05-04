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
        bool RemoveAll();
        void AddClusterRelationship(IServer parent, IServer child);
        List<IServer> GetClusterChildren(Guid parentId);
    }
} 