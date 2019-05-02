using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Converters;
using BinarApp.DesktopClient.Models;
using BinarApp.DesktopClient.Properties;
using BinarApp.DesktopClient.Units;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BinarApp.DesktopClient.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private NetworkUtils _networkUtils;
        private IProxyService<Fixation> _fixationProxyService;
        private EmployeePlateNumberService _srv;

        public List<IncidentItemViewModel> OriginalFixations;
        private List<IncidentItemViewModel> OriginalHistoryFixations;

        private string[] KeyWords =
        {
            "ориентировка",
            "грабеж",
            "разбой",
            "разбоя",
            "контрабанда",
            "контробанда",
            "задержать",
            "кража",
            "преступлен",
            "розыск",
            "уголовн",
            "угон"
        };

        private string _plateNumberTextSearch;
        public string PlateNumberTextSearch
        {
            get => _plateNumberTextSearch;
            set
            {

                if (_plateNumberTextSearch != value)
                {
                    _plateNumberTextSearch = value;
                    // Here logic for filter
                    if (!string.IsNullOrWhiteSpace(_plateNumberTextSearch))
                    {
                        var filteredCollection = OriginalFixations.Where(x => x.CarNumber.ToLower().Contains(_plateNumberTextSearch.ToLower())).ToList();
                        Incidents = new ObservableCollection<IncidentItemViewModel>(filteredCollection);
                    }
                    else
                    {
                        Incidents = new ObservableCollection<IncidentItemViewModel>(OriginalFixations);
                    }

                    RaisePropertyChanged("Incidents");
                    RaisePropertyChanged();
                }
            }
        }

        private string _descriptionTextSearch;
        public string DescriptionTextSearch
        {
            get => _descriptionTextSearch;
            set
            {

                if (_descriptionTextSearch != value)
                {
                    _descriptionTextSearch = value;
                    // Here logic for filter
                    if (!string.IsNullOrWhiteSpace(_descriptionTextSearch))
                    {
                        var filteredCollection = OriginalHistoryFixations.Where(x => x.Description.ToLower().Contains(_descriptionTextSearch.ToLower())).ToList();
                        IncidentHistory = new ObservableCollection<IncidentItemViewModel>(filteredCollection);
                    }
                    else
                    {
                        IncidentHistory = new ObservableCollection<IncidentItemViewModel>(OriginalHistoryFixations);
                    }

                    RaisePropertyChanged("IncidentHistory");
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _dateTimeFilter;
        public DateTime DateTimeFilter
        {
            get => _dateTimeFilter;
            set
            {
                if (_dateTimeFilter.Date != value.Date)
                {
                    _dateTimeFilter = value;
                    IncidentSelected = null;
                    LastIncident = null;
                    IsDetailOpen = false;
                    GetFixations();
                    RaisePropertyChanged();

                    //HistoryDateTimeFilter = _dateTimeFilter;
                    //RaisePropertyChanged("HistoryDateTimeFilter");
                }
            }
        }

        //private DateTime _historyDateTimeFilter;
        //public DateTime HistoryDateTimeFilter
        //{
        //    get => _historyDateTimeFilter;
        //    set
        //    {
        //        if (_historyDateTimeFilter != value)
        //        {
        //            _historyDateTimeFilter = value;

        //            GetIncidentHistory();

        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        private int _flayoutMode = 0;
        public int FlayoutMode
        {
            get { return _flayoutMode; }
            set
            {
                if (_flayoutMode != value)
                {
                    _flayoutMode = value;
                    if (_flayoutMode == 0)
                    {
                        FormPageVisible = Visibility.Hidden;
                        IncedentItemVisible = Visibility.Visible;
                    }
                    else
                    {
                        FormPageVisible = Visibility.Visible;
                        IncedentItemVisible = Visibility.Hidden;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        private Visibility _formPageVisible = Visibility.Hidden;
        public Visibility FormPageVisible
        {
            get { return _formPageVisible; }
            set
            {
                if (_formPageVisible != value)
                {
                    _formPageVisible = value;
                    RaisePropertyChanged();
                }
            }
        }
        private Visibility _incedentItemVisible = Visibility.Visible;
        public Visibility IncedentItemVisible
        {
            get { return _incedentItemVisible; }
            set
            {
                if (_incedentItemVisible != value)
                {
                    _incedentItemVisible = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isDetailOpen = false;
        public bool IsDetailOpen
        {
            get => _isDetailOpen;
            set
            {
                if (_isDetailOpen != value)
                {
                    _isDetailOpen = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isHistoryDownloading;
        public bool IsHistoryDownloading
        {
            get => _isHistoryDownloading;
            set
            {
                if (_isHistoryDownloading != value)
                {
                    _isHistoryDownloading = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isIncidentLoading;
        public bool IsIncidentLoading
        {
            get => _isIncidentLoading;
            set
            {
                if (_isIncidentLoading != value)
                {
                    _isIncidentLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<IncidentItemViewModel> Incidents { get; set; }

        public ObservableCollection<IncidentItemViewModel> IncidentHistory { get; set; }

        public MainPageViewModel(IProxyService<Fixation> fixationProxyService,
            EmployeePlateNumberService srv)
        {
            _networkUtils = new NetworkUtils();
            _fixationProxyService = fixationProxyService;
            _srv = srv;

            Incidents = new ObservableCollection<IncidentItemViewModel>();
            IncidentHistory = new ObservableCollection<IncidentItemViewModel>();
        }

        private IncidentItemViewModel _incidentSelected;
        public IncidentItemViewModel IncidentSelected
        {
            get => _incidentSelected;
            set
            {
                if (_incidentSelected != value)
                {
                    if (value != null && value.Status != "DarkGray")
                    {
                        _incidentSelected = value;
                        IncidentHistorySelected = value;
                        IsDetailOpen = true;

                        RaisePropertyChanged();
                        GetIncidentHistory();
                    }
                    else
                    {
                        _incidentSelected = null;
                        IsDetailOpen = false;
                        RaisePropertyChanged("IncidentSelected");               
                    }

                    LastIncident = value;
                }
            }
        }

        private IncidentItemViewModel _incidentHistorySelected;
        public IncidentItemViewModel IncidentHistorySelected
        {
            get => _incidentHistorySelected;
            set
            {
                if (_incidentHistorySelected != value)
                {
                    _incidentHistorySelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        private IncidentItemViewModel _lastIncident;
        public IncidentItemViewModel LastIncident
        {
            get => _lastIncident;
            set
            {
                if (_lastIncident != value)
                {
                    _lastIncident = value;
                    RaisePropertyChanged();
                }
            }
        }

        private async void GetFixations()
        {
            IsIncidentLoading = true;
            //Main.StatusBarMessage = "Производится загрузка данных";

            var date = DateTimeFilter;

            string query = $"/Fixations?$filter=FixationDate ge {date.ToString("yyyy-MM-dd")}T00:00:00Z and FixationDate le {date.ToString("yyyy-MM-dd")}T23:59:59Z and NickName eq '{_srv.PlateNumber}'";

            var data = await _fixationProxyService.GetCollection(query);

            var res = data.Select(x =>
            {
                var temp = data.Where(y => y.GRNZ == x.GRNZ)
                    .Where(y => !string.IsNullOrWhiteSpace(y.Description))
                    .Select(y => y.Description.ToLower());

                var hasCriticalIncidents = temp.Any(f => KeyWords.Any(y => f.Contains(y)));

                var imgPath = FixationImageConverter.ConvertBase64ToImage(x.Image);
                var imgSource = FixationImageConverter.ConvertToImageSource(imgPath);

                // Delete file
                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }

                return new
                {
                    x.Id,
                    CarNumber = x.GRNZ,
                    DateTime = x.FixationDate,
                    x.Description,
                    Speed = x.Speed.ToString(),
                    ImageSource = imgSource,
                    InSearch = hasCriticalIncidents,
                };
            }).ToList();

            //res.ForEach(item => 
            //{
            //    var hasIncidents = data.Where(x => x.GRNZ == item.CarNumber)
            //        .Where(x => !string.IsNullOrWhiteSpace(x.Description))
            //        .Select(x => x.Description.ToLower())
            //        .Any(x => KeyWords.Any(y => x.Contains(y)));

            //    item.InSearch = hasIncidents;
            //});

            Incidents = new ObservableCollection<IncidentItemViewModel>(res.Select(x =>
                new IncidentItemViewModel()
                {
                    Id = x.Id,
                    CarNumber = x.CarNumber,
                    DateTime = x.DateTime,
                    Description = x.Description,
                    ImageSource = x.ImageSource,
                    InSearch = x.InSearch,
                    Speed = x.Speed,
                    Status = x.InSearch ? "Red" : "Orange",
                    FlayoutText = x.InSearch ? "ВНИМАНИЕ, ТС В РОЗЫСКЕ!" : "Детальная информация"
                })
                .OrderByDescending(x => x.DateTime).ToList());

            // Make a full copy
            OriginalFixations = new List<IncidentItemViewModel>(Incidents);
            if (!string.IsNullOrWhiteSpace(PlateNumberTextSearch))
            {
                PlateNumberTextSearch = string.Empty;
            }

            if (Incidents.Any())
            {
                IncidentSelected = Incidents.First();
            }

            IsIncidentLoading = false;

            //Main.StatusBarMessage = "Загрузка данных завершена";
            RaisePropertyChanged("Incidents");
        }

        public void SetIncidentCollection(List<IncidentItemViewModel> incidents)
        {
            Incidents = new ObservableCollection<IncidentItemViewModel>(incidents);
            OriginalFixations = new List<IncidentItemViewModel>(Incidents);
            RaisePropertyChanged("Incidents");
        }

        private void GetIncidentHistory()
        {
            new Task(() =>
            {
                List<Fixation> fixations = null;
                IsHistoryDownloading = true;
                try
                {
                    _networkUtils.CheckConnection();
                    if (NetworkUtils.NetworkConnectionIsAvailable)
                    {
                        string query = $"/Fixations?$filter=GRNZ eq '{_incidentSelected.CarNumber}'";
                        fixations = _fixationProxyService.GetCollection(query).Result;
                    }

                    var vmCollection = ConvertIncidentHistoriesToViewModel(fixations.OrderByDescending(x => x.FixationDate).ToList());

                    // Make a full copy
                    OriginalHistoryFixations = new List<IncidentItemViewModel>(vmCollection);
                    if (!string.IsNullOrWhiteSpace(DescriptionTextSearch))
                    {
                        DescriptionTextSearch = string.Empty;
                    }

                    IncidentHistory = new ObservableCollection<IncidentItemViewModel>(vmCollection);
                    IncidentHistorySelected = IncidentHistory.FirstOrDefault(x => x.Id == IncidentHistorySelected.Id);
                    RaisePropertyChanged("IncidentHistory");
                    RaisePropertyChanged("IncidentHistorySelected");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Message: {ex.Message}, " +
                          $"stack trace: {ex.StackTrace}, " +
                          $"inner exception message: {ex.InnerException?.InnerException?.Message}");
                }
                finally
                {
                    IsHistoryDownloading = false;
                }
            }).Start();
        }

        private List<IncidentItemViewModel> ConvertIncidentHistoriesToViewModel(List<Fixation> collection)
        {
            var result = new List<IncidentItemViewModel>();

            if (collection == null)
                return result;

            result = collection.Select(item =>
            {
                bool inSearch = collection
                                .Where(x => x.GRNZ == item.GRNZ)
                                .Any(x => KeyWords.Any(y => !string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(y)));

                var imgPath = FixationImageConverter.ConvertBase64ToImage(item.Image);
                var imgSource = FixationImageConverter.ConvertToImageSource(imgPath);

                var res = new IncidentItemViewModel
                {
                    //FullName = $"{x.LastName} {x.FirstName} {x.MiddleName}",
                    Id = item.Id,
                    CarNumber = item.GRNZ ?? string.Empty,
                    DateTime = item.FixationDate,//.ToString("dd.MM.yyyy HH:mm:ss"),
                    Description = item.Description ?? string.Empty,
                    Speed = item.Speed.ToString(),
                    InSearch = inSearch,
                    ImageSource = imgSource
                    //Place = x.PUNKT ?? string.Empty
                };

                // Delete file
                if (File.Exists(imgPath))
                {
                    File.Delete(imgPath);
                }

                return res;
            })
            .ToList();

            return result;
        }
    }
}
