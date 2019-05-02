using BinarApp.DecktopApplication.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using System.Configuration;
using BinarApp.Core.Models;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using BinarApp.DecktopApplication.Proxies;
using BinarApp.Core.POCO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BinarApp.DecktopApplication.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// 
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Equipment _equipment;

        private LocationManager _locationManager;
        private FileProvider _fileProvider;
        private FixationIncidentProxy _fixationIncidentProxy;

        private FileDetailProvider _fileDetailProvider;

        private ProxyService<Equipment> _proxyService;
        private ProxyService<Fixation> _proxyFixationService;

        public SettingViewModel Settings { get; set; }
        
        public ObservableCollection<ObservingPlateViewModel> PlateCollection { get; set; }

        private ObservingPlateViewModel _currentPlate;
        public ObservingPlateViewModel CurrentPlate
        {
            get => _currentPlate;
            set
            {
                if (_currentPlate != value)
                {
                    _currentPlate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICollection<EquipmentMovement> EquipmentMovements { get; set; }

        public bool IsInEquipment { get; set; }

        public event EventHandler<EquipmentMovement> MovementInEquipmentChanged;

        public event EventHandler<EquipmentMovement> MovementInEquipmentFinished;

        public ICollection<IncidentSended> IncidentSendedCollection { get; set; }

        public RelayCommand TestAction { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(LocationManager locationManager, FileProvider fileProvider, FixationIncidentProxy fixationIncidentProxy, FileDetailProvider fileDetailProvider, ProxyService<Equipment> proxyService, ProxyService<Fixation> proxyFixationService)
        {
            Settings = new SettingViewModel();
            PlateCollection = new ObservableCollection<ObservingPlateViewModel>();
            _locationManager = locationManager;
            _fileProvider = fileProvider;
            _fixationIncidentProxy = fixationIncidentProxy;
            _locationManager.LocationChanged += _locationManager_LocationChanged;
            _fileDetailProvider = fileDetailProvider;
            _proxyService = proxyService;
            _proxyFixationService = proxyFixationService;
            //MovementInEquipmentChanged += MainViewModel_MovementInEquipmentChanged;
            //MovementInEquipmentFinished += MainViewModel_MovementInEquipmentFinished;

            EquipmentMovements = new List<EquipmentMovement>();
            IncidentSendedCollection = new List<IncidentSended>();
            IsInEquipment = false;
            
            TestAction = new RelayCommand(test);

            _locationManager.Start();
        }

        //private void MainViewModel_MovementInEquipmentFinished(object sender, EquipmentMovement e)
        //{
        //    var currentEquipmentMovements = EquipmentMovements
        //        .Where(x => x.Equipment.Id == e.Equipment.Id)
        //        .ToList();

        //    var allIncidents = new List<CsvRowModel>();

        //    foreach (var dates in currentEquipmentMovements)
        //    {
        //        var incidents = GetFilteredRows(dates.Start, dates.Finish.Value);
        //        allIncidents.AddRange(incidents);
        //    }

        //    var groupedByPlate = allIncidents.GroupBy(x => x.Plate);

        //    PlateCollection = new ObservableCollection<ObservingPlateViewModel>(groupedByPlate.Select(x => new ObservingPlateViewModel()
        //    {
        //        Plate = x.Key,
        //        Incidents = new ObservableCollection<ObservingPlateIncidentViewModel>(x.OrderBy(f => f.DateTime)
        //            .Select(f => new ObservingPlateIncidentViewModel()
        //            {
        //                Date = f.DateTime,
        //                Path = _fileProvider.ConvertFileNameToPath(f.FileName, f.DateTime)
        //            }))
        //    }));

        //    RaisePropertyChanged("PlateCollection");

        //    // send to server
        //    //Send(e.Equipment.Id, PlateCollection.ToList());
        //}

        //private void MainViewModel_MovementInEquipmentChanged(object sender, EquipmentMovement e)
        //{
        //    var allIncidents = GetFilteredRows(DateTime.Now);

        //    var groupedByPlate = allIncidents.GroupBy(x => x.Plate);

        //    PlateCollection = new ObservableCollection<ObservingPlateViewModel>(groupedByPlate.Select(x => new ObservingPlateViewModel()
        //    {
        //        Plate = x.Key,
        //        Incidents = new ObservableCollection<ObservingPlateIncidentViewModel>(x.OrderBy(f => f.DateTime)
        //            .Select(f => new ObservingPlateIncidentViewModel()
        //            {
        //                Date = f.DateTime,
        //                Path = _fileProvider.ConvertFileNameToPath(f.FileName, f.DateTime)
        //            }))
        //    }));

        //    RaisePropertyChanged("PlateCollection");
        //}

        private ICollection<CsvRowModel> GetFilteredRows(DateTime startDate, DateTime? finishDate = null)
        {
            var rows = _fileProvider.GetCollection(startDate);

            var filteredRows = rows.AsQueryable()
                .Where(x => x.DateTime >= startDate);

            if (finishDate.HasValue)
            {
                filteredRows = filteredRows.Where(x => x.DateTime <= finishDate.Value);
            }

            return rows.ToList();
        }
        
        private async Task Send(Equipment equipment, List<ObservingPlateViewModel> plateCollection)
        {
            //var date = DateTime.Now;
            var date = new DateTime(2019, 02, 05);

            var dateQ = string.Format("?$filter=FixationDate gt {0}T00:00:00.00Z and FixationDate lt {0}T23:59:59.00Z",
                date.ToString("yyyy-MM-dd"));

            //var dateQ = string.Empty;

            var fixations = await _proxyFixationService.GetCollection("Fixations", dateQ);

            var existingIncidents = fixations.Select(x => new
            {
                Plate = x.GRNZ,
                Year = x.FixationDate.Year,
                x.FixationDate.Month,
                x.FixationDate.Day,
                x.FixationDate,
                x.EquipmentId
            }).ToList();

            var res = new List<FixationIncidentViewModel>();
            
            foreach (var plate in plateCollection)
            {
                if (plate.Incidents.Count > 1)
                {
                    var plateIncidentsOrdered = plate.Incidents.OrderBy(x => x.Date).ToList();

                    var firstIncident = plate.Incidents.First();

                    if (!existingIncidents.Any(x => x.Plate == plate.Plate
                         && x.Year == firstIncident.Date.Year
                         && x.Month == firstIncident.Date.Month
                         && x.Day == firstIncident.Date.Day
                         && x.EquipmentId == equipment.Id))
                    {
                        var first = plateIncidentsOrdered.First();
                        var last = plateIncidentsOrdered.Last();

                        res.Add(new FixationIncidentViewModel()
                        {
                            EquipmentId = equipment.Id,
                            PlateNumber = plate.Plate,
                            DateStart = plateIncidentsOrdered.First().Date,
                            FirstImage = ConvertToBase64(plateIncidentsOrdered.First().Path),
                            FirstImagePlate = ConvertToBase64(plateIncidentsOrdered.First().PathToNumber),
                            //FirstImage = ConvertToBase64(@"C:\camera\2018-12-08\11\2018-12-08_11-50-02-666_A743FH-KAZ.jpg"),
                            DateFinish = plateIncidentsOrdered.Last().Date,
                            LastImage = ConvertToBase64(plateIncidentsOrdered.Last().Path),
                            LastImagePlate = ConvertToBase64(plateIncidentsOrdered.Last().PathToNumber),
                            //LastImage = ConvertToBase64(@"C:\camera\2018-12-08\11\2018-12-08_11-50-03-506_050ULA02-KAZ_CTX.jpg"),
                        });
                    }
                }
            }

            Settings.Sending = "Отправка началась";

            await _fixationIncidentProxy.Post(res);

            Settings.Sending = "Отправка кончилась";
        }

        private string ConvertToBase64(string path)
        {
            var pathStr = path;

            if (File.Exists(pathStr))
            {
                byte[] imageArray = File.ReadAllBytes(pathStr);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return base64ImageRepresentation;
            }
            else
            {
                var start = path.Substring(0, path.Length - 4);
                var end = path.Substring(path.Length - 4, 4);
                pathStr = string.Format("{0}{2}{1}", start, end, "_CTX");
            }

            if (File.Exists(pathStr))
            {
                byte[] imageArray = File.ReadAllBytes(pathStr);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return base64ImageRepresentation;
            }
            else
            {
                return string.Empty;
            }

        }

        private void _locationManager_LocationChanged(object sender, LocationEventArg e)
        {
            //TODO here need diss
            if (e.GeoCoordinate != null)
            {
                Settings.Longitude = e.GeoCoordinate.Longitude;
                Settings.Latitude = e.GeoCoordinate.Latitude;
            }
            else
            {
                Settings.Longitude = double.NegativeInfinity;
                Settings.Latitude = double.NegativeInfinity;
            }

            if (e.IsInEquipment)
            {
                Settings.EquipmentName = e.CurrentEquipment.Name;
                _equipment = e.CurrentEquipment;
                ShowPlates(_equipment, DateTime.Now);
            }
            else
            {
                Settings.EquipmentName = "Отсутствуют данные о местоположении";

                if (_equipment != null)
                {
                    // need send
                    var eqs = new Equipment()
                    {
                        Id = _equipment.Id,
                        GeoJson = _equipment.GeoJson
                    };

                    ShowPlates(_equipment, DateTime.Now);
                    Send(eqs, PlateCollection.ToList());
                }

                _equipment = null;
            }
            
            
        }

        private async Task ShowPlates(Equipment equipment, DateTime date)
        {
            var equip = new EquipmentPolygon(equipment);

            var data = _fileDetailProvider.GetCollection(date);

            var currentData = data
                .Where(x =>
                    (x.Latitude >= equip.BottomLeft.Lat && x.Longitude >= equip.BottomLeft.Lng)
                    && (x.Latitude <= equip.TopRight.Lat && x.Longitude <= equip.TopRight.Lng))
                .ToList();


            var groupedByPlate = currentData.GroupBy(x => x.Plate);

            PlateCollection = new ObservableCollection<ObservingPlateViewModel>(groupedByPlate.Select(x => new ObservingPlateViewModel()
            {
                Plate = x.Key,
                Incidents = new ObservableCollection<ObservingPlateIncidentViewModel>(x.Select(f =>
                {
                    var div = Convert.ToInt32((f.DateTime - x.Min(z => z.DateTime)).Minutes / 1);

                    return new
                    {
                        f.DateTime,
                        DivMin = div,
                        f.FileName
                    };
                })
                .GroupBy(f => f.DivMin)
                .Select(f =>
                {
                    var fileName = f.First().FileName;
                    var fDate = f.First().DateTime;

                    if (fileName.Contains("772LA02"))
                    {
                        Debug.WriteLine("DEBUG: {0}, {1}", f.Key, fDate);
                    }

                    var res = new ObservingPlateIncidentViewModel()
                    {
                        Date = fDate,
                        Path = _fileDetailProvider.ConvertFileNameToPath(fileName, fDate),
                        PathToNumber = _fileDetailProvider.ConvertFileNameToPathAndNumber(fileName, fDate)
                    };

                    return res;
                })
                .OrderBy(f => f.Date))
            }).OrderByDescending(x => x.Incidents.Count));

            RaisePropertyChanged("PlateCollection");
        }

        private int k = 0;

        private async void test()
        {
            var date = new DateTime(2019, 02, 27);

            var eqs = await _proxyService.GetCollection("Equipments");

            if (k >= eqs.Count)
            {
                k = 0;
            }

            var eq = eqs[k];

            await ShowPlates(eq, date);

            if (PlateCollection.Select(x => x.Incidents.Count).Any(f => f > 1))
            {
                await Send(eq, PlateCollection.ToList());
            }

            

            k++;

            //foreach (var eq in eqs)
            //{
            //    await ShowPlates(eq, date);

            //    k++;

            //    if (k == 1)
            //    {

            //    }

            //}

            //var data = _fileDetailProvider.GetCollection(date);

            //var groupedByPlate = data.GroupBy(x => x.Plate);

            //PlateCollection = new ObservableCollection<ObservingPlateViewModel>(groupedByPlate.Select(x => new ObservingPlateViewModel()
            //{
            //    Plate = x.Key,
            //    Incidents = new ObservableCollection<ObservingPlateIncidentViewModel>(x.Select(f =>
            //                {
            //                    var div = Convert.ToInt32((f.DateTime - x.Min(z => z.DateTime)).Minutes / 5);

            //                    return new
            //                    {
            //                        f.DateTime,
            //                        DivMin = div,
            //                        f.FileName
            //                    };
            //                })
            //                .GroupBy(f => f.DivMin )
            //                .Select(f =>
            //                {
            //                    var fileName = f.First().FileName;
            //                    var fDate = f.First().DateTime;

            //                    if (fileName.Contains("772LA02"))
            //                    {
            //                        Debug.WriteLine("DEBUG: {0}, {1}", f.Key, fDate );
            //                    }

            //                    var res = new ObservingPlateIncidentViewModel()
            //                    {
            //                        Date = fDate,
            //                        Path = _fileDetailProvider.ConvertFileNameToPath(fileName, fDate),
            //                        PathToNumber = _fileDetailProvider.ConvertFileNameToPathAndNumber(fileName, fDate)
            //                    };

            //                    return res;
            //                })
            //                .OrderBy(f => f.Date))
            //}).OrderByDescending(x => x.Incidents.Count));

            //RaisePropertyChanged("PlateCollection");








            //var equipments = await _proxyService.GetCollection("Equipments");
            //var equipPolygons = equipments.Select(x => new EquipmentPolygon(x)).ToList();

            //foreach (var equip in equipPolygons)
            //{
            //    if (equip.Equipment.Id == 1)
            //    {
            //        continue;
            //    }

            //    var data = _fileDetailProvider.GetCollection(date);

            //    var currentData = data
            //        .Where(x =>
            //            (x.Latitude >= equip.BottomLeft.Lat && x.Longitude >= equip.BottomLeft.Lng)
            //            && (x.Latitude <= equip.TopRight.Lat && x.Longitude <= equip.TopRight.Lng))
            //        .ToList();

            //    var groupedByPlate = currentData.GroupBy(x => x.Plate);

            //    PlateCollection = new ObservableCollection<ObservingPlateViewModel>(groupedByPlate.Select(x => new ObservingPlateViewModel()
            //    {
            //        Plate = x.Key,
            //        Incidents = new ObservableCollection<ObservingPlateIncidentViewModel>(x.OrderBy(f => f.DateTime)
            //            .Select(f => new ObservingPlateIncidentViewModel()
            //            {
            //                Date = f.DateTime,
            //                Path = _fileDetailProvider.ConvertFileNameToPath(f.FileName, f.DateTime)
            //            }))
            //    }));

            //    RaisePropertyChanged("PlateCollection");

            //    await Send(equip.Equipment, PlateCollection.ToList());
            //}

            //Settings.Sending = "Всё отправлено";

            //Send(1, PlateCollection.ToList());
        }
    }
}