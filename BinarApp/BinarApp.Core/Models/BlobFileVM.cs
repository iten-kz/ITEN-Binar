using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{
    public class BlobFileVM
    {
        public string Url { get; set; }
        public string BlobName { get; set; }
        public byte[] ImgArr { get; set; }
    }
}
