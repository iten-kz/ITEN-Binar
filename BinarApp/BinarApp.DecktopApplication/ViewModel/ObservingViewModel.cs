using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.ViewModel
{
    public class ObservingViewModel: ViewModelBase
    {
        public int EquipmentId { get; set; }
        
        private string _equipmentName;
        public string EquipmentName
        {
            get => _equipmentName;
            set { _equipmentName = value; RaisePropertyChanged(); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set { _date = value; RaisePropertyChanged(); }
        }

        private DateTime _lastDate;
        public DateTime LastDate
        {
            get => _lastDate;
            set { _lastDate = value; RaisePropertyChanged(); RaisePropertyChanged("DifferentWithLastTime"); }
        }

        public TimeSpan DifferentWithLastTime
        {
            get => _lastDate - DateTime.Now;
        }

        public ObservableCollection<ObservingItemViewModel> Collection { get; set; }

        private ObservingPlateViewModel _currentPlate;
        public ObservingPlateViewModel CurrentPlate 
        {
            get => _currentPlate;
            set { _currentPlate = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<ObservingPlateViewModel> PlateCollection { get; set; }

        public ObservingViewModel()
        {
            Date = DateTime.Now;
            LastDate = DateTime.Now;
            Collection = new ObservableCollection<ObservingItemViewModel>();
            PlateCollection = new ObservableCollection<ObservingPlateViewModel>();
        }

        

    }
}
