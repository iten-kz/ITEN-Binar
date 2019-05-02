using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.ViewModel
{
    public class FixationItemViewModel: ViewModelBase
    {
        private string _plateNumber;
        public string PlateNumber
        {
            get => _plateNumber;
            set
            {
                if (_plateNumber != value)
                {
                    _plateNumber = value;
                    RaisePropertyChanged();
                }
            }
        }


        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}
