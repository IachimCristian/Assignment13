using System;
using InfraSim.Models.Db;
using InfraSim.Models.Server;
using Xunit;

namespace InfraSim.Tests
{
    public class DatabaseTest
    {
        DbServer Server;
        InfraSimContext Context;
        IRepositoryFactory Factory;
        IUnitOfWork UnitOfWork;
        IRepository<DbServer> ServerRepository;

        public DatabaseTest() // Preparation for the test 
        {
            Server = new DbServer { // Initializing the server for the test 
                Id = Guid.NewGuid(),
                ServerType = ServerType.Server
            };

            Context = new MemoryInfraSimContext();
            Context.Database.EnsureCreated();

            Factory = new RepositoryFactory(Context);
            UnitOfWork = new UnitOfWork(Context, Factory);

            ServerRepository = UnitOfWork.GetRepository<DbServer>();

            UnitOfWork.Begin();
            ServerRepository.Insert(Server);
            UnitOfWork.SaveChanges();
        }

        [Fact]
        public void WhenAddingServersInDatabase_TheyAreStoredIfSuccess() // Implemented the success transaction test 
        {
            UnitOfWork.Commit();

            var servers = ServerRepository.GetAll();
            Assert.Single(servers);
        }

        [Fact]
        public void WhenAddingServersInDatabase_TheyAreNotStoredIfFailed()  
        {
            UnitOfWork.Rollback();

            var servers = ServerRepository.GetAll();
            Assert.Empty(servers);
        }
    }
} 