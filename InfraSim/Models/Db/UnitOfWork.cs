using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("InfraSim.Tests")]

namespace InfraSim.Models.Db
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction? Transaction { get; set; }
        private readonly IServiceProvider _serviceProvider;
        private InfraSimContext? _context;
        private IRepositoryFactory? _repositoryFactory;
        private readonly IDictionary<Type, object> Repositories = new Dictionary<Type, object>();

        private InfraSimContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<InfraSimContext>();
                }
                return _context;
            }
        }

        private IRepositoryFactory RepositoryFactory
        {
            get
            {
                if (_repositoryFactory == null)
                {
                    _repositoryFactory = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRepositoryFactory>();
                }
                return _repositoryFactory;
            }
        }

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Internal constructor for testing
        internal UnitOfWork(InfraSimContext context, IRepositoryFactory repositoryFactory)
        {
            _context = context;
            _repositoryFactory = repositoryFactory;
            _serviceProvider = null!; // Won't be used in testing scenario
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : DbItem
        {
            if (!Repositories.ContainsKey(typeof(TEntity)))
            {
                Repositories[typeof(TEntity)] = RepositoryFactory.Create<TEntity>();
            }

            return (IRepository<TEntity>)Repositories[typeof(TEntity)];
        }

        public void Begin()
        {
            Transaction = Context.Database.BeginTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
            Transaction = null;
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            Transaction = null;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            _context?.Dispose();
        }
    }
} 