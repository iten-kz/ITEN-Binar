using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Models
{
    public interface IProxyService<TEntity> : IDisposable
    {
        Task<List<TEntity>> GetCollection(string filterQuery = "");

        Task PostEntity(TEntity entity);
    }
}
