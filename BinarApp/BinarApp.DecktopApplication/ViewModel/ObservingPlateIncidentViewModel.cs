using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.ViewModel
{
    public class ObservingPlateIncidentViewModel: ViewModelBase
    {
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { _date = value; RaisePropertyChanged(); }
        }

        private string _path;
        public string Path
        {
            get => _path;
            set { _path = value; RaisePropertyChanged(); }
        }

        private string _pathToNumber;
        public string PathToNumber
        {
            get => _pathToNumber;
            set { _pathToNumber = value; RaisePropertyChanged(); }
        }
    }
}
