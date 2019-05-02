using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.Models
{
    public class IncidentManager: IDisposable
    {
        private FileDetailProvider _fileProvider;

        public IncidentManager(FileDetailProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public ICollection<CsvRowDetailModel> GetCollection(DateTime date)
        {
            var data = _fileProvider.GetCollection(date);

            var result = new List<CsvRowDetailModel>();
            
            // here need load from API
            // we need send only NEW



            return result;
        }


        public void Dispose()
        {
            if (_fileProvider != null)
            {
                _fileProvider.Dispose();
                _fileProvider = null;
            }
        }
    }
}
