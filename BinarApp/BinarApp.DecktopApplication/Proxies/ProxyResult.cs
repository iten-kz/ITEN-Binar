using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Proxies
{
    public class ProxyResult<TEntity> where TEntity: class
    {
        public List<TEntity> Value { get; set; }
    }
}
