using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.ViewModel
{
    public class ObservingItemViewModel: ViewModelBase
    {
        private DateTime _start;
        public DateTime Start
        {
            get => _start;
            set { _start = value; RaisePropertyChanged(); }
        }

        private DateTime _finish;
        public DateTime Finish
        {
            get => _finish;
            set { _finish = value; RaisePropertyChanged(); }
        }

        public ObservingItemViewModel()
        {
            Start = DateTime.Now;
            Finish = DateTime.Now;
        }

    }
}
