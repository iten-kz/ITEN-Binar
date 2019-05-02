using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DecktopApplication.ViewModel
{
    public class SettingViewModel: ViewModelBase
    {
        private string _equipmentName;
        public string EquipmentName
        {
            get => _equipmentName;
            set { _equipmentName = value; RaisePropertyChanged(); }
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set { _longitude = value; RaisePropertyChanged(); }
        }

        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set { _latitude = value; RaisePropertyChanged(); }
        }

        private string _sending;
        public string Sending
        {
            get => _sending;
            set { _sending = value; RaisePropertyChanged(); }
        }
    }
}
