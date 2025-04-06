using System;

namespace InfraSim.Models.Db
{
    public interface IDbItem
    {
        Guid Id { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
} 