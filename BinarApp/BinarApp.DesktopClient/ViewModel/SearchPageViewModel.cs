using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Converters;
using BinarApp.DesktopClient.Models;
using BinarApp.DesktopClient.Units;
using GalaSoft.MvvmLight;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.ViewModel
{
    public class SearchPageViewModel : ViewModelBase
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private NetworkUtils _networkUtils;
        private IProxyService<Fixation> _fixationProxyService;

        public ObservableCollection<IncidentItemViewModel> Incidents { get; set; }

        private List<IncidentItemViewModel> OriginalFixations;

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

        private bool _isCollectionDownloading;
        public bool IsCollectionDownloading
        {
            get => _isCollectionDownloading;
            set
            {
                if (_isCollectionDownloading != value)
                {
                    _isCollectionDownloading = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        private DateTime _dateTimeFilterFrom;
        public DateTime DateTimeFilterFrom
        {
            get => _dateTimeFilterFrom;
            set
            {
                if (_dateTimeFilterFrom.Date != value.Date)
                {
                    _dateTimeFilterFrom = value;
                    GetFixations();
                    RaisePropertyChanged();
                    RaisePropertyChanged("PlateNumberTextSearch");
                }
            }
        }

        private DateTime _dateTimeFilterTo;
        public DateTime DateTimeFilterTo
        {
            get => _dateTimeFilterTo;
            set
            {
                if (_dateTimeFilterTo.Date != value.Date)
                {
                    _dateTimeFilterTo = value;
                    GetFixations();
                    RaisePropertyChanged();
                }
            }
        }

        private IncidentItemViewModel _incidentSelected;
        public IncidentItemViewModel IncidentSelected
        {
            get => _incidentSelected;
            set
            {
                if (_incidentSelected != value)
                {
                    _incidentSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        private EmployeePlateNumberService _srv;
        public SearchPageViewModel(IProxyService<Fixation> fixationProxyService,
           EmployeePlateNumberService srv)
        {
            _networkUtils = new NetworkUtils();
            _fixationProxyService = fixationProxyService;
            _srv = srv;

            var today = DateTime.Today;
            DateTimeFilterFrom = new DateTime(today.Year, today.Month, 1);
            DateTimeFilterTo = today;

            Incidents = new ObservableCollection<IncidentItemViewModel>();
        }

        public async void GetFixations()
        {
            IsCollectionDownloading = true;

            var dateFrom = DateTimeFilterFrom;
            var dateTo = DateTimeFilterTo;

            string query = $"/Fixations?$filter=FixationDate ge {DateTimeFilterFrom.ToString("yyyy-MM-dd")}T00:00:00Z and FixationDate le {DateTimeFilterTo.ToString("yyyy-MM-dd")}T23:59:59Z";

            try
            {
                var data = await _fixationProxyService.GetCollection(query);
                var res = data.Select(x =>
                {
                    var temp = data.Where(y => y.GRNZ == x.GRNZ)
                        .Where(y => !string.IsNullOrWhiteSpace(y.Description))
                        .Select(y => y.Description.ToLower());

                    var hasIncidents = temp.Any(f => KeyWords.Any(y => f.Contains(y)));

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
                        InSearch = hasIncidents
                    };
                }).ToList();

                Incidents = new ObservableCollection<IncidentItemViewModel>(res.Select(x =>
                    new IncidentItemViewModel()
                    {
                        Id = x.Id,
                        CarNumber = x.CarNumber,
                        DateTime = x.DateTime,
                        Description = x.Description,
                        ImageSource = x.ImageSource,
                        InSearch = x.InSearch,
                        Speed = x.Speed
                    })
                    .OrderByDescending(x => x.DateTime).ToList());

                // Make a full copy
                OriginalFixations = new List<IncidentItemViewModel>(Incidents);
                //if (!string.IsNullOrWhiteSpace(PlateNumberTextSearch))
                //{
                //    PlateNumberTextSearch = string.Empty;
                //}

                RaisePropertyChanged("PlateNumberTextSearch");

                if (Incidents.Any())
                {
                    IncidentSelected = Incidents.First();
                }

                IsCollectionDownloading = false;
                RaisePropertyChanged("Incidents");
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                     $"stack trace: {ex.StackTrace}, " +
                     $"inner exception message: {ex.InnerException?.InnerException?.Message}");
            }
        }
    }
}
