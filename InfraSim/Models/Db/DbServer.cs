using System;
using InfraSim.Models.Server;

namespace InfraSim.Models.Db
{
    public class DbServer : DbItem
    {
        public ServerType ServerType { get; set; }
        public Guid? ParentId { get; set; }
    }
} 