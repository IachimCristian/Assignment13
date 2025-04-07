using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using InfraSim.Models.Db;

namespace InfraSim.Tests
{
    public class MemoryInfraSimContext : InfraSimContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options) // Overrides the configuration of the DbContext to use an in-memory SQLite database 
        {
            SqliteConnection connection = new SqliteConnection("DataSource=:memory:"); // Create a new in-memory SQLite connection 
            connection.Open(); // Must be opened explicitly to remain available during the context lifetime 
            options.UseSqlite(connection); // Configure EF Core to use the in-memory SQLite connection
        }
    }
} 