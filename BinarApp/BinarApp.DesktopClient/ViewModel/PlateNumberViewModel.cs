using BinarApp.DesktopClient.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BinarApp.DesktopClient.ViewModel
{
    public class PlateNumberViewModel : ViewModelBase
    {
        private EmployeePlateNumberService _service;
        private MainPageViewModel _mainPageViewModel;

        private Visibility _isEmpty = Visibility.Hidden;
        public Visibility IsEmpty
        {
            get => _isEmpty;
            set
            {
                if (_isEmpty != value)
                {
                    _isEmpty = value;
                    RaisePropertyChanged();
                }
            }
        }


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
                    //IsEmpty = string.IsNullOrWhiteSpace(_plateNumber);
                }
            }
        }

        public RelayCommand SendPlateNumber { get; set; }

        public event EventHandler PlateNumberSaved;

        public PlateNumberViewModel(EmployeePlateNumberService epnServer,
            MainPageViewModel mainPageViewModel)
        {
            _service = epnServer;
            _mainPageViewModel = mainPageViewModel;

            SendPlateNumber = new RelayCommand(SavePlateNumber);

            PlateNumber = _service.PlateNumber;
        }

        private void SavePlateNumber()
        {
            _service.PlateNumber = PlateNumber;

            var str = _service.PlateNumber;

            if (!string.IsNullOrWhiteSpace(str))
            {
                IsEmpty = Visibility.Hidden;
                PlateNumberSaved(null, new EventArgs());
                _mainPageViewModel.DateTimeFilter = DateTime.Today;
            }
            else
            {
                IsEmpty = Visibility.Visible;
            }
        }
    }
}
