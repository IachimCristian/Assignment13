using System;
using System.Collections.Generic;
using System.Linq;
using InfraSim.Models.Db;
using InfraSim.Models.Capability;

namespace InfraSim.Models.Server
{
    public class ServerDataMapper : IServerDataMapper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICapabilityFactory _capabilityFactory;

        public ServerDataMapper(IUnitOfWork unitOfWork, ICapabilityFactory capabilityFactory)
        {
            _unitOfWork = unitOfWork;
            _capabilityFactory = capabilityFactory;
        }

        public List<IServer> GetAll()
        {
            var dbServers = _unitOfWork.GetRepository<DbServer>().GetAll();
            var builder = new ServerBuilder();
            var servers = new List<IServer>();
            foreach (var db in dbServers)
            {
                servers.Add(builder
                    .WithId(db.Id)
                    .WithType(db.ServerType)
                    .WithCapability(_capabilityFactory.Create(db.ServerType))
                    .Build());
            }
            return servers;
        }

        public IServer? Get(Guid id)
        {
            var db = _unitOfWork.GetRepository<DbServer>().Get(id);
            if (db == null) return null;
            return new ServerBuilder()
                .WithId(db.Id)
                .WithType(db.ServerType)
                .WithCapability(_capabilityFactory.Create(db.ServerType))
                .Build();
        }

        public void Insert(IServer server)
        {
            var db = new DbServer { Id = server.Id, ServerType = server.ServerType };
            _unitOfWork.GetRepository<DbServer>().Insert(db);
            _unitOfWork.SaveChanges();
        }

        public void Remove(IServer server)
        {
            var repo = _unitOfWork.GetRepository<DbServer>();
            var db = repo.Get(server.Id);
            if (db != null)
            {
                repo.Delete(db);
                _unitOfWork.SaveChanges();
            }
        }
    }
} 