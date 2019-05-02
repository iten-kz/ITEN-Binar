using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.ViewModel
{
    public class ObservingPlateViewModel: ViewModelBase
    {
        private string _plate;
        public string Plate
        {
            get => _plate;
            set { _plate = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<ObservingPlateIncidentViewModel> Incidents { get; set; }

        public ObservingPlateViewModel()
        {
            Incidents = new ObservableCollection<ObservingPlateIncidentViewModel>();
        }

    }
}
