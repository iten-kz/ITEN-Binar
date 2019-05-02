using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Managers;
using BinarApp.DesktopClient.Models;
using DevExpress.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.ViewModel
{
    public class ParkingViewModel: ViewModelBase
    {
        private IProxyService<Fixation> _fixationProxyService;
        private EmployeePlateNumberService _srv;
        private TattileCameraManager _tattileCameraManager;
        private IProxyService<Equipment> _equipmentProxyService;

        public RelayCommand CreateCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand ContinueCommand { get; set; }
        
        private ParkingFixationViewModel _fixationSelected;
        public ParkingFixationViewModel FixationSelected
        {
            get => _fixationSelected;
            set
            {
                if (_fixationSelected != value)
                {
                    _fixationSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ParkingFixationViewModel> ParkingFixations { get; set; }

        private List<Equipment> _equipmentCollection;

        public ParkingViewModel(IProxyService<Fixation> fixationProxyService,
            EmployeePlateNumberService srv,
            TattileCameraManager tattileCameraManager,
            IProxyService<Equipment> equipmentProxyService)
        {
            _fixationProxyService = fixationProxyService;
            _srv = srv;
            _tattileCameraManager = tattileCameraManager;
            _equipmentProxyService = equipmentProxyService;

            CreateCommand = new RelayCommand(Create);
            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            ContinueCommand = new RelayCommand(Continue);

            ParkingFixations = new ObservableCollection<ParkingFixationViewModel>();
            
        }

        private async void GetEquipments()
        {
            _equipmentCollection = await _equipmentProxyService.GetCollection("/Equipments");
        }

        private void Create()
        {
            var item = new ParkingFixationViewModel();
            ParkingFixations.Add(item);
            FixationSelected = item;
        }

        private void Start()
        {
            FixationSelected.StartDate = DateTime.Now;
        }

        private void Stop()
        {
            var storyDate = DateTime.Now;

            var tattileImageNames = _tattileCameraManager.GetImages(storyDate);

            // Step 4
            var tattileFilteredImages = _tattileCameraManager
                .FilterImagesByDateTime(tattileImageNames, FixationSelected.StartDate, storyDate);

            var fixations = tattileFilteredImages.Select(x => new FixationItemViewModel()
            {
                PlateNumber = x

            }).ToList();

            if (!FixationSelected.ContinueDate.HasValue)
            {
                FixationSelected.First = new ObservableCollection<FixationItemViewModel>(fixations);
            }
            else
            {
                FixationSelected.FinishDate = DateTime.Now;
                FixationSelected.Second = new ObservableCollection<FixationItemViewModel>(fixations);
            }
        }

        private void Continue()
        {
            FixationSelected.ContinueDate = DateTime.Now;
        }

    }
}
