using BinarApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Interfaces
{
    public interface IBlobStorageProvider
    {
        BlobFileVM Upload(string base64, string iin);
        BlobFileVM GetByBlobName(string blobName);
    }
}
