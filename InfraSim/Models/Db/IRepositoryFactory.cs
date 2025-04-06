namespace InfraSim.Models.Db
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> Create<TEntity>() where TEntity : DbItem;
    }
} 