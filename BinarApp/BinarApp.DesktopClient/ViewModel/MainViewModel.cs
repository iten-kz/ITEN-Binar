using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Converters;
using BinarApp.DesktopClient.Managers;
using BinarApp.DesktopClient.Models;
using BinarApp.DesktopClient.Units;
using BinarApp.DesktopClient.Views.Pages;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace BinarApp.DesktopClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private IFolderListenerProvider _folderListener;
        private IDataCacheProvider _dataCacheProvider;

        private BinarCameraManager _binarCameraManager;
        private TattileCameraManager _tattileCameraManager;
        private FileManager _fileManager;
        private IProxyService<Fixation> _fixationProxyService;

        private NetworkUtils _networkUtils;
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

        private Logger _logger; // = LogManager.GetCurrentClassLogger();

        public string ApplicationTitle { get; set; } = "Road anti-speed patrolling intellectual device";

        private bool _plateNumberFound;
        public bool PlateNumberFound
        {
            get => _plateNumberFound;
            set
            {
                if (_plateNumberFound != value)
                {
                    _plateNumberFound = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _statusBarMessage;
        public string StatusBarMessage
        {
            get => _statusBarMessage;
            set
            {
                if (_statusBarMessage != value)
                {
                    _statusBarMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        private UserControl _currentPage;
        public UserControl CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isInternetConnection;
        public bool IsInternetConnected
        {
            get => _isInternetConnection;
            set
            {
                if (_isInternetConnection != value)
                {
                    _isInternetConnection = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isBinarCameraConnection;
        public bool IsBinarCameraConnected
        {
            get => _isBinarCameraConnection;
            set
            {
                if (_isBinarCameraConnection != value)
                {
                    _isBinarCameraConnection = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isIIPCameraConnection;
        public bool IsIPCameraConnected
        {
            get => _isIIPCameraConnection;
            set
            {
                if (_isIIPCameraConnection != value)
                {
                    _isIIPCameraConnection = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _mainModeIsActive;
        public string MainModeIsActive
        {
            get => _mainModeIsActive;//!_counterModeIsActive && !_nightModeIsActive;
            set
            {
                if (_mainModeIsActive != value)
                {
                    _mainModeIsActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _counterModeIsActive;
        public bool CounterModeIsActive
        {
            get => _counterModeIsActive;//!_mainModeIsActive && !_nightModeIsActive;
            set
            {
                if (_counterModeIsActive != value)
                {
                    _counterModeIsActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _streamSource;
        public string StreamSource
        {
            get => _streamSource;
            set
            {
                if (_streamSource != value)
                {
                    _streamSource = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _nightModeIsActive;
        public string NightModeIsActive
        {
            get => _nightModeIsActive; //!_mainModeIsActive && !_counterModeIsActive;
            set
            {
                if (_nightModeIsActive != value)
                {
                    _nightModeIsActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isPlateFormOpen;
        public bool IsPlateFormOpen
        {
            get => _isPlateFormOpen;
            set
            {
                if (_isPlateFormOpen != value)
                {
                    _isPlateFormOpen = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _searchButtonBackground;
        public string SearchButtonBackground
        {
            get => _searchButtonBackground;
            set
            {
                if (_searchButtonBackground != value)
                {
                    _searchButtonBackground = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _formButtonBackground;
        public string FormButtonBackground
        {
            get => _formButtonBackground;
            set
            {
                if (_formButtonBackground != value)
                {
                    _formButtonBackground = value;
                    RaisePropertyChanged();
                }
            }
        }

        private LanguageItemViewModel _selectedLanguage;
        public LanguageItemViewModel SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;

                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (config.AppSettings.Settings["SYSTEM_LANGUAGE"] == null)
                    {
                        config.AppSettings.Settings.Add("SYSTEM_LANGUAGE", _selectedLanguage.Value);
                    }
                    else
                    {
                        config.AppSettings.Settings["SYSTEM_LANGUAGE"].Value = _selectedLanguage.Value;
                    }
                    config.Save();
                    ConfigurationManager.RefreshSection("appSettings");

                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<LanguageItemViewModel> Languages { get; set; }

        public MainPageViewModel MainPageViewModel { get; set; }
        public SearchPageViewModel SearchPageViewModel { get; set; }
        public PlateNumberViewModel PlateNumberViewModel { get; set; }
        public ParkingViewModel ParkingViewModel { get; set; }

        public RelayCommand GoToMainPage { get; set; }
        public RelayCommand GoToSearchPage { get; set; }
        public RelayCommand GoToFormPage { get; set; }
        public RelayCommand GoToVideoPage { get; set; }

        public RelayCommand SwitchMainMode { get; set; }
        public RelayCommand SwitchNightMode { get; set; }

        public RelayCommand ChangeLanguage { get; set; }
        public RelayCommand Logout { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(MainPageViewModel mainPageViewModel,
            SearchPageViewModel searchPageViewModel,
            PlateNumberViewModel plateNumberViewModel,
            BinarCameraManager binarCameraManager,
            TattileCameraManager tattileCameraManager,
            FileManager fileManager,
            IProxyService<Fixation> fixationProxyService,
            IFolderListenerProvider folderListener,
            FormPageViewModel formPage,
            IDataCacheProvider dataCacheProvider,
            ParkingViewModel parkingViewModel)
        {
            _formPage = formPage;
            MainPageViewModel = mainPageViewModel;
            SearchPageViewModel = searchPageViewModel;
            PlateNumberViewModel = plateNumberViewModel;
            ParkingViewModel = parkingViewModel;

            _binarCameraManager = binarCameraManager;
            _tattileCameraManager = tattileCameraManager;
            _fileManager = fileManager;
            _fixationProxyService = fixationProxyService;

            _folderListener = folderListener;
            _dataCacheProvider = dataCacheProvider;

            _networkUtils = new NetworkUtils();

            if (IsInDesignMode)
            {
                IsInternetConnected = true;
                IsBinarCameraConnected = true;
                IsIPCameraConnected = true;
            }
            else
            {
                IsInternetConnected = true;
                IsBinarCameraConnected = true;
                IsIPCameraConnected = true;
            }

            GoToMainPage = new RelayCommand(GoToMain);
            GoToSearchPage = new RelayCommand(GoToSearch);
            GoToVideoPage = new RelayCommand(GoToVideo);
            GoToFormPage = new RelayCommand(GoToForm);

            SwitchMainMode = new RelayCommand(SwitchMainModeHandler);
            SwitchNightMode = new RelayCommand(SwitchNightModeModeHandler);

            //UnitlsConfiguration();

            ChangeLanguage = new RelayCommand(() =>
            {
                //string culture = "ru-RU";
                //Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                //Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            });

            Logout = new RelayCommand(() =>
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            });

            IsPlateFormOpen = true;
            PlateNumberViewModel.PlateNumberSaved += PlateNumberViewModel_PlateNumberSaved;

            Languages = new ObservableCollection<LanguageItemViewModel>
            {
                new LanguageItemViewModel { Name = "Рус", Value = "ru-RU" },
                new LanguageItemViewModel { Name = "Қаз", Value = "kk-KZ" },
                new LanguageItemViewModel { Name = "Eng", Value = "en-EN" }
            };

            var confLang = ConfigurationManager.AppSettings["SYSTEM_LANGUAGE"];
            if (confLang != null)
            {
                _selectedLanguage = Languages.FirstOrDefault(x => x.Value == confLang);
            }

            SearchButtonBackground = "#1878BA";
            FormButtonBackground = "#1878BA";
            PlateNumberFound = false;
            MainModeIsActive = "True";
            NightModeIsActive = "True";

            //GoToParking();

            _logger = LogManager.GetCurrentClassLogger();
        }

        private FormPageViewModel _formPage;
        public FormPageViewModel FormPage
        {
            get { return _formPage; }
        }

        private void PlateNumberViewModel_PlateNumberSaved(object sender, EventArgs e)
        {
            IsPlateFormOpen = false;
        }

        #region Buttons click handlers

        public bool MainModeOn { get; set; }
        public bool NightModeOn { get; set; }

        private void SwitchMainModeHandler()
        {
            //if (MainModeIsActive == "False")
            //{
            //    MainModeIsActive = "True";
            //}
            //else
            //{
            //    MainModeIsActive = "False";
            //}

            MainModeOn = !MainModeOn;

            //MainModeIsActive = !MainModeIsActive;
            //NightModeIsActive = false;
            if (MainModeOn)
            {
                NightModeIsActive = "False";
                StatusBarMessage = "Режим патрулирования активирован";
                //_folderListener.StartListener();
            }
            else
            {
                StatusBarMessage = "Режим патрулирования отключен";
                NightModeIsActive = "True";
                //_folderListener.StopListener();
            }
        }

        private void SwitchNightModeModeHandler()
        {
            //if (NightModeIsActive == "False")
            //{
            //    NightModeIsActive = "True";
            //}
            //else
            //{
            //    NightModeIsActive = "False";
            //}

            NightModeOn = !NightModeOn;

            //NightModeIsActive = !NightModeIsActive;
            //MainModeIsActive = false;
            if (NightModeOn)
            {
                MainModeIsActive = "False";
                StatusBarMessage = "Режим сверки активирован";
                _folderListener.StartListener();
            }
            else
            {
                MainModeIsActive = "True";
                StatusBarMessage = "Режим сверки отключен";
                _folderListener.StopListener();
            }
        }

        public void GoToMain()
        {
            GoToParking();
            //var mainPageView = SimpleIoc.Default.GetInstance<MainPageView>();
            //CurrentPage = mainPageView;
            //PageTitle = "Главная";
        }

        public void GoToSearch()
        {
            //GoToParking();
            //PlateNumberFound = false;
            //SearchButtonBackground = "#1878BA";
            ////SearchPageViewModel.GetFixations();

            //var searchPageView = SimpleIoc.Default.GetInstance<SearchPageView>();
            //CurrentPage = searchPageView;
            //PageTitle = "История";
        }

        public void GoToForm()
        {
            //GoToParking();
            //MainPageViewModel.FlayoutMode = 1;
            //MainPageViewModel.IsDetailOpen = true;
            //FormButtonBackground = "#1878BA";
            //var formPageView = SimpleIoc.Default.GetInstance<FormPageView>();
            //CurrentPage = formPageView;
            //PageTitle = "Ручной ввод";
        }

        private void GoToVideo()
        {
            //GoToParking();
            //var mainPageView = SimpleIoc.Default.GetInstance<VideoPageView>();
            //CurrentPage = mainPageView;
            //PageTitle = "Видео";
        }

        private void GoToParking()
        {
            var parkingView = SimpleIoc.Default.GetInstance<ParkingPageView>();

            if (CurrentPage != parkingView)
            {
                CurrentPage = parkingView;
                PageTitle = "Парковки";
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Tattile event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DataReceivedDisplayHandler(object sender, FileReceivedEventArgs args)
        {
            // Need to test
            //Thread.Sleep(300);

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (obj, e) =>
            {
                try
                {
                    MainPageViewModel.IsIncidentLoading = true;
                    var pathArgs = args.FilePath.Split('\\');
                    var path = pathArgs[pathArgs.Length - 1];

                    // Check for duplicates
                    var existedIncidents = MainPageViewModel.Incidents.ToList();
                    var carNumber = path.Split('_')[2].Split('-')[0];

                    StatusBarMessage = "Получено событие от Tattile";
                    _logger.Info($"Start handling tattile file: {args.FilePath}");

                    DateTime storyDate = DateTime.Now;
                    string fileNameArg = args.FileName.Split('_')[2];
                    string plateNumber = fileNameArg.Split('-')[0];

                    //string plateNumber = fileNameArg.Substring(2, fileNameArg.Length - 6);

                    if (!string.IsNullOrWhiteSpace(plateNumber))
                    {
                        SearchButtonBackground = "#5EBA7D";
                        PlateNumberFound = true;

                        var dateTo = DateTime.Today;
                        var dateFrom = new DateTime(dateTo.Year - 5, 6, 1);

                        SearchPageViewModel.DateTimeFilterFrom = dateFrom;
                        SearchPageViewModel.DateTimeFilterTo = dateTo;
                        SearchPageViewModel.PlateNumberTextSearch = plateNumber;
                        RaisePropertyChanged("DateTimeFilterTo");

                        // Add to collection 
                        //var tattileImageNames = _tattileCameraManager.GetImages(storyDate);

                        //var tattileFilteredImages = _tattileCameraManager
                        //    .FilterImagesByDateTime(tattileImageNames, storyDate);

                        //string tattileTotalFilteredImage = _tattileCameraManager
                        //    .GetFilteredImageName(tattileFilteredImages, storyDate);

                        //string tattileTotalFilteredImage = tattileImageNames.LastOrDefault();

                        Fixation tattileFixation = _tattileCameraManager.GetFixation(path);

                        // Add to UI                   
                        var imgBase64 = FixationImageConverter.ConvertBase64ToImage(tattileFixation.Image);
                        var imgSrs = FixationImageConverter.ConvertToImageSource(imgBase64);

                        var vmCollection = MainPageViewModel.Incidents.ToList();
                        var status = MainPageViewModel.OriginalFixations.FirstOrDefault(x => x.CarNumber == plateNumber);

                        vmCollection.Insert(0, new IncidentItemViewModel
                        {
                            DateTime = tattileFixation.FixationDate,
                            Speed = "-",
                            CarNumber = $"{tattileFixation.GRNZ}",// + ({vmCollection.Count + 1})", // Temp
                            ImageSource = imgSrs,
                            Description = string.Empty,
                            Status = status == null ? "DarkGray" : status.Status,
                            FlayoutText = (status != null && status.Status == "Red") ? "ВНИМАНИЕ, ТС В РОЗЫСКЕ!" : "Детальная информация"
                        });

                        // Delete file
                        if (File.Exists(imgBase64))
                        {
                            File.Delete(imgBase64);
                        }

                        if (existedIncidents.Any(x => x.CarNumber == carNumber))
                        {
                            // Delete duplicate
                            existedIncidents = existedIncidents.Where(x => x.CarNumber != carNumber).ToList();
                            existedIncidents.Insert(0, vmCollection.First());
                            MainPageViewModel.SetIncidentCollection(existedIncidents);
                        }
                        else
                        {
                            MainPageViewModel.SetIncidentCollection(vmCollection);
                        }

                        
                        //MainPageViewModel.LastIncident = MainPageViewModel.Incidents.First();

                        RaisePropertyChanged("LastIncident");

                        //if (MainPageViewModel.LastIncident.Status != "DarkGray")
                        //{
                        MainPageViewModel.IncidentSelected = MainPageViewModel.Incidents.First(); //MainPageViewModel.LastIncident;
                        // }
                    }

                    StatusBarMessage = "Внимание, произведена сверка!";

                }
                catch (Exception ex)
                {
                    _logger.Error($"Message: {ex.Message}, " +
                        $"stack trace: {ex.StackTrace}, " +
                        $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                    MainPageViewModel.IsIncidentLoading = false;
                }
            };
            bw.RunWorkerCompleted += (obj, e) =>
            {
                MainPageViewModel.IsIncidentLoading = false;
                bw.Dispose();
                bw = null;
            };
            bw.RunWorkerAsync();
        }

        /// <summary>
        /// Main BINAR handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void FileReceivedHandler(object sender, FileReceivedEventArgs args)
        {
            try
            {
                MainPageViewModel.IsIncidentLoading = true;
                StatusBarMessage = "Получено событие от Бинар";
                _logger.Info($"Start handling file: {args.FilePath}");

                string xmlData = _fileManager.ReadFile(args.FilePath);
                StatusBarMessage = "Файл открыт Бинар";
                StoryList storyList = _binarCameraManager.ConvertFromStringXml(xmlData);
                StatusBarMessage = "Разбор файла Бинар";

                // Step 1
                DateTime storyDate = DateTime.Now; //_binarCameraManager.GetDateTime(storyList);
                //storyDate = new DateTime(2018, 6, 16, 13, 5, 55);

                // Step 2
                Fixation binarFixation = _binarCameraManager.GetIndexedFixation(storyDate, storyList);
                StatusBarMessage = "Фиксации определены";

                _logger.Info($"Binar fixation generated from: {args.FileName}; speed: {binarFixation.Speed}; date: {binarFixation.FixationDate}");

                // Step 3
                var tattileImageNames = _tattileCameraManager.GetImages(storyDate);
                StatusBarMessage = "Изображения определены";

                // Step 4
                var tattileFilteredImages = _tattileCameraManager
                    .FilterImagesByDateTime(tattileImageNames, storyDate);
                StatusBarMessage = "Сортировка изображений";

                // Step 5
                string tattileTotalFilteredImage = _tattileCameraManager
                    .GetFilteredImageName(tattileFilteredImages, storyDate);
                StatusBarMessage = "Изображение найдено";

                // Step 6
                Fixation tattileFixation = _tattileCameraManager.GetFixation(tattileTotalFilteredImage);

                // Step 7 - Send to DB or cache
                Fixation mergedFixation = new Fixation
                {
                    Speed = binarFixation.Speed,
                    FixationDate = binarFixation.FixationDate,
                    GRNZ = string.Empty,
                    Image = binarFixation.Image,
                    BirthDate = DateTime.Now,
                    PenaltySum = 0.0M
                };

                if (tattileFixation != null)
                {
                    _logger.Info($"Tattile fixation generated from: {args.FileName}; plate number: {tattileFixation.GRNZ}; date: {tattileFixation.FixationDate}");
                    mergedFixation.GRNZ = tattileFixation.GRNZ;
                    mergedFixation.Image = tattileFixation.Image;
                }

                // Is online mode
                // bool sendResult = false;
                _networkUtils.CheckConnection();
                Task postFixationTask = null;
                if (NetworkUtils.NetworkConnectionIsAvailable)
                {
                    postFixationTask = _fixationProxyService.PostEntity(mergedFixation);
                }
                else
                {
                    _dataCacheProvider.SaveDataToFile(new List<Fixation> { mergedFixation }, CacheDataType.BinarCache);
                    StatusBarMessage = "Кеширование отправки данных";
                }

                // Step 8 - Add to UI                   
                var imgPath = FixationImageConverter.ConvertBase64ToImage(mergedFixation.Image);
                var imgSrs = FixationImageConverter.ConvertToImageSource(imgPath);
                StatusBarMessage = "Преобразование картинки";

                var vmCollection = MainPageViewModel.Incidents.ToList();
                vmCollection.Insert(0, new IncidentItemViewModel
                {
                    DateTime = mergedFixation.FixationDate,
                    Speed = mergedFixation.Speed.ToString(),
                    CarNumber = mergedFixation.GRNZ,
                    ImageSource = imgSrs,
                    Description = string.Empty
                });

                MainPageViewModel.SetIncidentCollection(vmCollection);
                MainPageViewModel.IncidentSelected = MainPageViewModel.Incidents.First();

                await postFixationTask;

                StatusBarMessage = "Обработка завершена";
                MainPageViewModel.IsIncidentLoading = false;
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                    $"stack trace: {ex.StackTrace}, " +
                    $"inner exception message: {ex.InnerException?.InnerException?.Message}");
            }
        }

        #endregion

        private void UnitlsConfiguration()
        {
            StreamSource = ConfigurationManager.AppSettings["STREAM_URL"].ToString();

            NetworkUtils.NetworkStatusChanged += NetworkUtils_NetworkStatusChanged;

            // Binar main handler
            _folderListener.FileReceived += DataReceivedDisplayHandler; // FileReceivedHandler;

            // Tattile tcp listener
            // Browser settings
            //var settings = new CefSettings();
            //settings.CefCommandLineArgs.Add("disable-gpu", "1");
            //settings.CachePath = "cache";
            //Cef.Initialize(settings);
        }

        private void NetworkUtils_NetworkStatusChanged(object sender, NetworkStatusEventArgs e)
        {
            IsInternetConnected = e.IsAvailable;
        }
    }
}