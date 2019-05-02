using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class CsvReader : IDisposable
    {
        private CultureInfo _provider = CultureInfo.InvariantCulture;

        public ICollection<CsvRowModel> ReadFile(string path)
        {
            var result = new List<CsvRowModel>();

            try
            {
                var data = File.ReadAllLines(path);

                result = data.Skip(1).Select(row => ConvertRowToModel(row))
                    .ToList();

            }
            catch (Exception ex)
            {

            }
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">Example: "tattile_1714000009;532WYA02-KAZ;6;2018-12-08;11-45-06-447;2018-12-08_11-45-06-447_532WYA02-KAZ.jpg"</param>
        /// <returns></returns>
        public CsvRowModel ConvertRowToModel(string row)
        {
            var columns = row.Split(';');

            // example: "2018-12-08"
            var dateStr = columns[3];
            // example: "11-45-06-447"
            var timeStr = columns[4].Substring(0, 8);

            var parseFormat = "yyyy-MM-dd-HH-mm-ss";
            
            // example result = "08-12-2018-11-45-06-447"
            var dateTimeStr = string.Format("{0}-{1}",
                dateStr,
                timeStr);

            var dateTime = DateTime.ParseExact(dateTimeStr, parseFormat, _provider);

            var res = new CsvRowModel()
            {
                Plate = columns[1],
                FileName = columns[5],
                Value = row,
                DateTime = dateTime
            };
            return res;
        }

        public void Dispose()
        {
            _provider = null;
        }
    }
}
