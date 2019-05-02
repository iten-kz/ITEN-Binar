using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class CsvRowModel
    {
        /// <summary>
        /// Index 1
        /// </summary>
        public string Plate { get; set; }

        /// <summary>
        /// Index 3-4
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Index 5
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// all row
        /// </summary>
        public string Value { get; set; }
    }
}
