using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class FileProvider : IDisposable
    {
        private CsvReader _csvReader;
        private string _generalPath;

        public FileProvider(CsvReader csvReader)
        {
            _generalPath = ConfigurationManager.AppSettings["TATTILE_FILE_PATH"].ToString();
            _csvReader = csvReader;
        }

        public ICollection<CsvRowModel> GetCollection(DateTime date)
        {
            var path = GetFullPathName(_generalPath, date);

            return _csvReader.ReadFile(path);
        }

        public string ConvertFileNameToPath(string fileName, DateTime date)
        {
            return string.Format("{0}\\{1}\\{2}\\{3}", 
                _generalPath, 
                date.ToString("yyyy-MM-dd"),
                date.ToString("HH"),
                fileName);
        }

        public string GetFullPathName(string generalPath, DateTime date)
        {
            return string.Format("{0}\\{1}\\{1}.{2}",
                generalPath,
                date.ToString("yyyy-MM-dd"),
                "csv");
        }

        public void Dispose()
        {
            _csvReader.Dispose();
            _csvReader = null;
        }
    }
}
