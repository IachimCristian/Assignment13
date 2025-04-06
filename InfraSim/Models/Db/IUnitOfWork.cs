using System;

namespace InfraSim.Models.Db
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : DbItem;

        void Begin();
        void Commit();
        void Rollback();

        void SaveChanges();
    }
} 