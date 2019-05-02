using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class CsvRowDetailModel
    {
        /// <summary>
        /// Index 2
        /// </summary>
        public string Plate { get; set; }

        /// <summary>
        /// Index 0-1
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Index 3-2
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Index 7
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Index 8
        /// </summary>
        public float Longitude { get; set; }
        
        /// <summary>
        /// all row
        /// </summary>
        public string Value { get; set; }
    }
}
