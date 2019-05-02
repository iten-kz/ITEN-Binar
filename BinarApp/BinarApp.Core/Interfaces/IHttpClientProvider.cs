using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Interfaces
{
    public interface IHttpClientProvider
    {
        Task<List<Fixation>> GetFixationsAsync(string queryString = "");
        Task<bool> SendFixationAsync(Fixation fixation);
        Task<List<Intruder>> GetIntrudersAsync(string queryString = "");
    }
}
