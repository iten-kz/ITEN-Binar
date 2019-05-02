using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class CsvDetailReader: IDisposable
    {
        private CultureInfo _provider = CultureInfo.InvariantCulture;
        
        public ICollection<CsvRowDetailModel> ReadFile(string path)
        {
            var result = new List<CsvRowDetailModel>();

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
        /// <param name="row">Example: "2018-12-08;11-52-16-694;A261YSN;2018-12-08_11-52-16-694_A261YSN-KAZ.jpg;2018-12-08/11;NA;UNKNOWN;+43.2245522;+76.9104156;532"</param>
        /// <returns></returns>
        public CsvRowDetailModel ConvertRowToModel(string row)
        {
            var columns = row.Split(';');

            // example: "2018-12-08"
            var dateStr = columns[0];
            // example: "11-45-06-447"
            var timeStr = columns[1].Substring(0, 8);

            var parseFormat = "yyyy-MM-dd-HH-mm-ss";

            // example result = "08-12-2018-11-45-06-447"
            var dateTimeStr = string.Format("{0}-{1}",
                dateStr,
                timeStr);

            var dateTime = DateTime.ParseExact(dateTimeStr, parseFormat, _provider);

            var path = columns[4].Split('/');

            var ltdString = columns[7].Substring(1).Replace('.', ',');
            var lngString = columns[8].Substring(1).Replace('.', ',');

            var res = new CsvRowDetailModel()
            {
                Plate = columns[2],
                FileName = path[1] + "/" + columns[3],
                Value = row,
                DateTime = dateTime,
                Latitude = Convert.ToSingle(ltdString),
                Longitude = Convert.ToSingle(lngString)
            };

            return res;
        }

        public void Dispose()
        {
            _provider = null;
        }

    }
}
