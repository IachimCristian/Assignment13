using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[assembly: InternalsVisibleTo("InfraSim.Tests")]

namespace InfraSim.Models.Db
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction? Transaction { get; set; }
        public readonly InfraSimContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(InfraSimContext context, IRepositoryFactory repositoryFactory)
        {
            _context = context;
            _repositoryFactory = repositoryFactory;
            _repositories = new Dictionary<Type, object>();
            _serviceProvider = null!;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : DbItem
        {
            if (!_repositories.ContainsKey(typeof(TEntity)))
            {
                _repositories[typeof(TEntity)] = _repositoryFactory.Create<TEntity>();
            }

            return (IRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        public void Begin()
        {
            Transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                SaveChanges();
                Transaction?.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error committing transaction: {ex.Message}");
                Rollback();
                throw;
            }
            finally
            {
                Transaction = null;
            }
        }

        public void Rollback()
        {
            try
            {
                Transaction?.Rollback();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling back transaction: {ex.Message}");
            }
            finally
            {
                Transaction = null;
            }
        }

        public void SaveChanges()
        {
            try
            {
                Console.WriteLine("UnitOfWork: About to save changes to database");
                
                _context.ChangeTracker.DetectChanges();
                
                var pendingChanges = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || 
                           e.State == EntityState.Added || 
                           e.State == EntityState.Deleted)
                    .ToList();
                
                if (pendingChanges.Any())
                {
                    foreach (var entry in pendingChanges)
                    {
                        Console.WriteLine($"UnitOfWork: Pending change - Entity: {entry.Entity.GetType().Name}, State: {entry.State}, ID: {(entry.Entity as DbItem)?.Id}");
                    }
                    
                    int changes = _context.SaveChanges();
                    Console.WriteLine($"UnitOfWork: SaveChanges completed - {changes} entities affected");
                    
                    if (changes != pendingChanges.Count)
                    {
                        Console.WriteLine($"WARNING: Expected to save {pendingChanges.Count} changes but only {changes} were saved!");
                    }
                    
                    foreach (var entry in pendingChanges)
                    {
                        if (entry.Entity is DbItem item)
                        {
                            var repo = this.GetRepository<DbItem>();
                            var savedItem = repo.Get(item.Id);
                            if (savedItem != null)
                            {
                                Console.WriteLine($"Verified: Entity with ID {item.Id} exists in database after save");
                            }
                            else if (entry.State != EntityState.Deleted)
                            {
                                Console.WriteLine($"ERROR: Entity with ID {item.Id} NOT FOUND in database after save!");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("UnitOfWork: No pending changes to save");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Concurrency conflict detected: {ex.Message}");
                Console.WriteLine($"UnitOfWork ERROR: Concurrency conflict: {ex.Message}");
                foreach (var entry in ex.Entries)
                {
                    try
                    {
                        entry.Reload();
                    }
                    catch (Exception reloadEx)
                    {
                        Console.WriteLine($"Error reloading entry: {reloadEx.Message}");
                    }
                }
                
                _context.ChangeTracker.DetectChanges();
                int changes = _context.SaveChanges();
                Console.WriteLine($"UnitOfWork: SaveChanges retry completed - {changes} entities affected");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving changes: {ex.Message}");
                Console.WriteLine($"UnitOfWork ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            _context.Dispose();
        }
    }
} 