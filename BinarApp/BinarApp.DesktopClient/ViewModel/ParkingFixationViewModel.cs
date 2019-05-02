
using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.ViewModel
{
    public class ParkingFixationViewModel: ViewModelBase
    {
        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime? _continueDate;
        public DateTime? ContinueDate
        {
            get => _continueDate;
            set
            {
                if (_continueDate != value)
                {
                    _continueDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime? _finishDate;
        public DateTime? FinishDate
        {
            get => _finishDate;
            set
            {
                if (_finishDate != value)
                {
                    _finishDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _equipmentName;
        public string EquipmentName
        {
            get => _equipmentName;
            set
            {
                if (_equipmentName != value)
                {
                    _equipmentName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TimeSpan DiffTime
        {
            get 
            {
                var toDate = FinishDate.HasValue ? FinishDate.Value : DateTime.Now;
                return toDate - StartDate;
            }
        }

        public ObservableCollection<FixationItemViewModel> First { get; set; }

        public ObservableCollection<FixationItemViewModel> Second { get; set; }
        
        public ObservableCollection<FixationItemViewModel> Result
        {
            get => new ObservableCollection<FixationItemViewModel>(First.Join(Second,
                f => f.PlateNumber,
                s => s.PlateNumber,
                (f, s) => s)); 
        }

        public ParkingFixationViewModel()
        {
            _startDate = DateTime.Now;
            First = new ObservableCollection<FixationItemViewModel>();
            Second = new ObservableCollection<FixationItemViewModel>();
        }

    }
}
