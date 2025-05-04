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
            try
            {
                var entities = Context.Set<TEntity>().ToList();
                Console.WriteLine($"Repository: Retrieved {entities.Count} entities of type {typeof(TEntity).Name}");
                return entities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository ERROR in GetAll for {typeof(TEntity).Name}: {ex.Message}");
                return new List<TEntity>();
            }
        }

        public TEntity Get(Guid id)
        {
            try
            {
                var entity = Context.Set<TEntity>()?.FirstOrDefault(x => x.Id == id);
                if (entity != null)
                {
                    Console.WriteLine($"Repository: Found entity {typeof(TEntity).Name} with ID {id}");
                }
                else
                {
                    Console.WriteLine($"Repository: Entity {typeof(TEntity).Name} with ID {id} NOT FOUND");
                }
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository ERROR in Get for {typeof(TEntity).Name} with ID {id}: {ex.Message}");
                return null;
            }
        }

        public void Insert(TEntity item)
        {
            try
            {
                // Check if entity is already being tracked
                var existing = Context.Set<TEntity>().Local.FirstOrDefault(e => e.Id == item.Id);
                if (existing != null)
                {
                    Console.WriteLine($"Repository: Entity {typeof(TEntity).Name} with ID {item.Id} already being tracked");
                    return;
                }
                
                Context.Set<TEntity>().Add(item);
                Console.WriteLine($"Repository: Added entity {typeof(TEntity).Name} with ID {item.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Repository ERROR in Insert for {typeof(TEntity).Name} with ID {item.Id}: {ex.Message}");
                throw;
            }
        }

        public void Update(TEntity item)
        {
            var existingEntry = Context.Entry(item);
            if (existingEntry.State == EntityState.Detached)
            {
                // If entity is not being tracked, attach it first
                var attachedEntity = Context.Set<TEntity>().Local.FirstOrDefault(e => e.Id == item.Id);
                if (attachedEntity != null)
                {
                    // Update values from item to attached entity
                    Context.Entry(attachedEntity).CurrentValues.SetValues(item);
                }
                else
                {
                    // Attach and mark as modified
                    Context.Set<TEntity>().Attach(item);
                    Context.Entry(item).State = EntityState.Modified;
                }
            }
            else
            {
                // Entity is being tracked, mark as modified
                existingEntry.State = EntityState.Modified;
            }
        }

        public void Delete(TEntity item)
        {
            // Handle potentially detached entities
            var existingEntry = Context.Entry(item);
            if (existingEntry.State == EntityState.Detached)
            {
                // If it's detached, find the attached version if it exists
                var attachedEntity = Context.Set<TEntity>().Local.FirstOrDefault(e => e.Id == item.Id);
                if (attachedEntity != null)
                {
                    Context.Remove(attachedEntity);
                }
                else
                {
                    // If not in local tracking, attach and remove
                    Context.Set<TEntity>().Attach(item);
                    Context.Remove(item);
                }
            }
            else
            {
                // Entity is being tracked, remove it
                Context.Remove(item);
            }
        }
    }
} 