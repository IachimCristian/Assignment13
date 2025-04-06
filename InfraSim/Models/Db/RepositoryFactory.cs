namespace InfraSim.Models.Db
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly InfraSimContext Context;

        public RepositoryFactory(InfraSimContext context)
        {
            Context = context;
        }

        public IRepository<TEntity> Create<TEntity>() where TEntity : DbItem
        {
            return new Repository<TEntity>(Context);
        }
    }
} 