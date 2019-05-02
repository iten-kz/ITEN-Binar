using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class FileDetailProvider : IDisposable
    {
        private CsvDetailReader _csvReader;
        private string _generalPath;

        public FileDetailProvider(CsvDetailReader csvReader)
        {
            _generalPath = ConfigurationManager.AppSettings["TATTILE_FILE_PATH"].ToString();
            _csvReader = csvReader;
        }

        public ICollection<CsvRowDetailModel> GetCollection(DateTime date)
        {
            var files = GetAllFiles(_generalPath, date);

            var result = new List<CsvRowDetailModel>();

            foreach (var path in files)
            {
                var data = _csvReader.ReadFile(path);
                result.AddRange(data);
            }
            
            return result;
        }

        public ICollection<string> GetAllFiles(string generalPath, DateTime date)
        {
            var path = string.Format("{0}\\{1}", 
                _generalPath, 
                date.ToString("yyyy-MM-dd"));

            var directoryInfo = new DirectoryInfo(path);

            var allFiles = directoryInfo.GetFiles("*.csv")
                .Select(x => x.FullName)
                .ToList();

            var filteredFiles = allFiles
                .OrderBy(x => x.Length)
                .Skip(1)
                .ToList();

            return filteredFiles;
        }

        public string ConvertFileNameToPath(string fileName, DateTime date)
        {
            var index = fileName.IndexOf('.');
            var name = fileName.Insert(index, "_CTX");

            return string.Format("{0}\\{1}\\{2}",
                _generalPath,
                date.ToString("yyyy-MM-dd"),
                name);
        }

        public string ConvertFileNameToPathAndNumber(string fileName, DateTime date)
        {
            return string.Format("{0}\\{1}\\{2}",
                _generalPath,
                date.ToString("yyyy-MM-dd"),
                fileName);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
