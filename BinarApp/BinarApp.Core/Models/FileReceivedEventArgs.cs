using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{
    /// <summary>
    /// Параметры события
    /// </summary>
    public class FileReceivedEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
