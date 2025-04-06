using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace InfraSim.Models.Db
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : DbItem
    {
        private readonly InfraSimContext Context;

        public Repository(InfraSimContext context)
        {
            Context = context;
        }

        public List<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public TEntity Get(Guid id)
        {
            return Context.Set<TEntity>()?.FirstOrDefault(x => x.Id == id);
        }

        public void Insert(TEntity item)
        {
            Context.Set<TEntity>().Add(item);
        }

        public void Update(TEntity item)
        {
            Context.Entry(item).State = EntityState.Modified;
        }

        public void Delete(TEntity item)
        {
            Context.Remove(item);
        }
    }
} 