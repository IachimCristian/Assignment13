using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InfraSim.Models.Db
{
    public class MemoryInfraSimContext : InfraSimContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            options.UseSqlite(connection);
        }
    }
} 