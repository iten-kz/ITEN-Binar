using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using BinarApp.Jobs;
using BinarApp.Utils;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;


namespace BinarApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    // TODO: Add manual data insert
    // TODO: Save images to blob storage instead of saving base64 to DB
    // TODO: Save video fragments to blob storage and display in UI
    // TODO: Get Long and Lat for map markers and current vehicle speed
    // TODO: Implement logic for live streaming from camera
    public partial class MainWindow : Window
    {
        private bool isStarted;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private IFolderListenerProvider _folderListener;
        private ITcpListenerProvider _tcpListener;
        private IHttpClientProvider _httpClientProvider;
        private IDataCacheProvider _dataCacheProvider;
        private IJobScheduler _jobScheduler;
        private IDataCacheJobListener _jobListener;

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

        // Confirm dialog params
        private string _plateNumber;
        private double _averageSpeed;
        private DateTime _dateKey;
        private string _base64Image;
        private string _xmlFileName;
        private string _xmlFilePath;

        private List<DialogFixationVM> _dialogFixationItems;

        public MainWindow(IFolderListenerProvider folderListener,
            ITcpListenerProvider tcpListener,
            //INetworkUtils networkUtils,
            IHttpClientProvider httpClientProvider,
            IDataCacheProvider dataCacheProvider,
            IJobScheduler jobScheduler,
            IDataCacheJobListener jobListener)
        {
            InitializeComponent();

            _folderListener = folderListener;
            _tcpListener = tcpListener;
            _httpClientProvider = httpClientProvider;
            _dataCacheProvider = dataCacheProvider;
            _jobScheduler = jobScheduler;
            _jobListener = jobListener;

            _jobScheduler.Start();

            _networkUtils = new NetworkUtils();
            isStarted = false;

            mainPage.btnStartMainMode.Click += btnStartMainMode_Click;
            mainPage.btnStartNightMode.Click += btnStartNightMode_Click;
            mainPage.btnStartCounterMode.Click += btnStartCounterMode_Click;
            searchPage.btnFind.Click += btnFind_Click;

            NetworkUtils.NetworkStatusChanged += (sender, args) => //async (sender, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (args.IsAvailable)
                    {
                        sbiConnectionStatus.Background = Brushes.LightGreen;
                        lblConnectionStatus.Text = "Соединение с интернетом установлено";
                    }
                    else
                    {
                        sbiConnectionStatus.Background = Brushes.LightCoral;
                        lblConnectionStatus.Text = "Нет соединения с интернетом";
                    }
                });

                // TODO: test
                try
                {
                    // Get data from cache
                    /*
                    var data = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.BinarCache) ??
                        new List<Fixation>();

                    if (data.Any() && args.IsAvailable)
                    {
                        // Save to DB
                        foreach (var item in data)
                        {
                            var result = await _httpClientProvider.SendFixationAsync(item);
                            // Delete item from cache
                            if (result)
                            {
                                _dataCacheProvider.RemoveItemFromCache<Fixation>(CacheDataType.BinarCache, item);
                            }
                        }
                    }
                    */
                }
                catch (Exception ex)
                {
                    _logger.Error($"Message: {ex.Message}, " +
                        $"stack trace: {ex.StackTrace}, " +
                        $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                    //MessageBox.Show(ex.Message);
                }
            };

            // Add from DB or cache
            _jobListener.DataCachingExecuted += (sender, args) =>
            {
                var fixations = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.DatabaseCache);
                if (fixations != null && fixations.Count > 0)
                {
                    var items = fixations.OrderByDescending(x => x.FixationDate).Take(20).ToList();

                    // TEMP
                    //for (int i = 0; i < items.Count(); i++)
                    //{
                    //    var item = items[i];
                    //    item.Description = "Угон";
                    //}

                    Dispatcher.Invoke(() =>
                    {
                        if (items.Any())
                        {
                            var first = items.First();
                            BitmapImage bi = ConvertBase64ToImage(string.Empty); //image base64
                            mainPage.imgMain.Source = bi;
                            mainPage.lblMainPlateNumber.Content = first.GRNZ;

                            bool inSearch = fixations
                                   .Where(x => x.GRNZ == first.GRNZ)
                                   .Any(x => KeyWords.Any(y => !string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(y)));

                            mainPage.lblWarning.Content = inSearch ? "TC в розыске!" : string.Empty;
                            mainPage.lblWarning.Background = inSearch ? Brushes.Red : Brushes.LightGray;

                            //if (!string.IsNullOrWhiteSpace(first.Description) &&
                            //    KeyWords.Any(x => first.Description.ToLower().Contains(x)))
                            //{
                            //    mainPage.lblWarning.Content = "Внимание, ТС находится в розыске!";
                            //    mainPage.lblWarning.Background = Brushes.Red;
                            //}
                            // Display speed
                            mainPage.lblSpeed.Content = $"{first.Speed.ToString()} КМ/Ч";
                            // Display plate number
                            mainPage.lblPlateNumber.Content = first.GRNZ;
                            // Display time
                            mainPage.lblTime.Content = first.FixationDate.ToString("HH:mm:ss");
                            mainPage.lblDate.Content = first.FixationDate.ToString("dd-MM-yyyy");

                            // Add to ribbon
                            var itemsSource = items.Select(x => new ListViewEventArgs
                            {
                                Image = ConvertBase64ToImage(string.Empty), //image base64
                                PlateNumber = x.GRNZ,
                                Description = x.Description ?? string.Empty,
                                //WarningText = !string.IsNullOrWhiteSpace(x.Description)
                                //    && KeyWords.Any(y => x.Description.ToLower().Contains(y)) ? "TC в розыске!" : string.Empty,
                                //WarningColor = !string.IsNullOrWhiteSpace(x.Description)
                                //    && KeyWords.Any(y => x.Description.ToLower().Contains(y)) ? Brushes.Red : Brushes.LightGray,
                                Speed = x.Speed,
                                FixationDate = x.FixationDate
                            }).ToList();

                            foreach (var item in itemsSource)
                            {
                                inSearch = fixations
                                    .Where(x => x.GRNZ == item.PlateNumber)
                                    .Any(x => KeyWords.Any(y => !string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(y)));

                                item.WarningText = inSearch ? "TC в розыске!" : string.Empty;
                                item.WarningColor = inSearch ? Brushes.Red : Brushes.LightGray;
                            }

                            mainPage.listImagesRibbon.ItemsSource = itemsSource;
                        }
                    });

                    //foreach (var item in items)
                    //{
                    //    string base64 = item.Image;
                    //    if (!string.IsNullOrWhiteSpace(base64))
                    //    {
                    //        Dispatcher.Invoke(() => 
                    //        {
                    //            BitmapImage bi = ConvertBase64ToImage(base64);
                    //            if (items.IndexOf(item) == 0)
                    //            {
                    //                mainPage.imgMain.Source = bi;
                    //                mainPage.lblMainPlateNumber.Content = item.GRNZ;
                    //            }
                    //        });
                    //    }
                    //}
                }
            };

            //mainPage.listImagesRibbon.Items.Add(@"/Media/Images/binar2.JPG");
            //wbSample.Navigate("http://195.93.152.93:8080/flu/StrobeMediaPlayback.swf");
        }

        #region Event handlers

        private async void DataReceivedDisplayHandler(object sender, TcpListenerEventArgs args)
        {
            try
            {
                var xml = args.Data;
                var serializer = new XmlSerializer(typeof(root));
                root item = null;
                using (TextReader reader = new StringReader(xml))
                {
                    item = (root)serializer.Deserialize(reader);
                }

                var fixations = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.DatabaseCache);
                _networkUtils.CheckConnection();

                if (fixations.Count == 0)
                {
                    if (NetworkUtils.NetworkConnectionIsAvailable)
                    {
                        string query = $"/Fixations?$filter=GRNZ eq '{item.PLATE_STRING}'&$orderby=FixationDate desc";
                        fixations = await _httpClientProvider.GetFixationsAsync(query);
                    }
                    else
                    {
                        fixations = new List<Fixation>();
                    }
                }
                else
                {
                    fixations = fixations.Where(x => x.GRNZ == item.PLATE_STRING).ToList();
                }

                mainPage.Dispatcher.Invoke(() =>
                {
                    bool inSearch = fixations
                        .Where(x => x.GRNZ == item.PLATE_STRING)
                        .Any(x => KeyWords.Any(y => !string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(y)));

                    //item.WarningText = inSearch ? "TC в розыске!" : string.Empty;
                    //item.WarningColor = inSearch ? Brushes.Red : Brushes.LightGray;
                    if (inSearch)
                    {
                        if (fixations.Count > 0 && !FixationsPopup.IsOpen)
                        {
                            _dialogFixationItems = fixations.Select(x => new DialogFixationVM
                            {
                                //FullName = $"{x.Intruder.LastName} {x.Intruder.FirstName} {x.Intruder.MiddleName}",
                                GRNZ = x.GRNZ ?? string.Empty,
                                FixationDate = x.FixationDate.ToString("dd.MM.yyyy HH:mm:ss"),
                                Description = x.Description ?? string.Empty,
                                Speed = x.Speed,
                                Image = ConvertBase64ToImage(string.Empty), //TODO image base64
                                Place = x.PUNKT ?? string.Empty
                            }).ToList();
                            dgFixations.ItemsSource = _dialogFixationItems;

                            FixationsPopup.IsOpen = true;
                        }
                    }
                });             
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                     $"stack trace: {ex.StackTrace}, " +
                     $"inner exception message: {ex.InnerException?.InnerException?.Message}");

            }

            //List<Fixation> fixations = null;
            //try
            //{
            //    var xml = args.Data;
            //    var serializer = new XmlSerializer(typeof(root));
            //    root item = null;
            //    using (TextReader reader = new StringReader(xml))
            //    {
            //        item = (root)serializer.Deserialize(reader);
            //    }

            //    _networkUtils.CheckConnection();
            //    fixations = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.DatabaseCache);
            //    if (fixations.Count == 0)
            //    {
            //        if (NetworkUtils.NetworkConnectionIsAvailable)
            //        {
            //            string query = $"/Fixations?$filter=GRNZ eq '{item.PLATE_STRING}'&$orderby=FixationDate desc";
            //            fixations = await _httpClientProvider.GetFixationsAsync(query);
            //        }
            //        else
            //        {
            //            fixations = new List<Fixation>();
            //        }
            //    }
            //    else
            //    {
            //        fixations = fixations.Where(x => x.GRNZ == item.PLATE_STRING).ToList();
            //    }

            //    if (fixations.Count > 0 && !FixationsPopup.IsOpen)
            //    {
            //        dgFixations.Dispatcher.Invoke(() =>
            //        {
            //            var data = fixations.Select(x => new
            //            {
            //                FullName = $"{x.LastName} {x.FirstName} {x.MiddleName}",
            //                GRNZ = x.GRNZ,
            //                FixationDate = x.FixationDate.ToString("dd.MM.yyyy HH:mm:ss"),
            //                Description = x.Description,
            //                Speed = x.Speed
            //            }).ToList();

            //            dgFixations.ItemsSource = data;
            //            FixationsPopup.IsOpen = true;
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error($"Message: {ex.Message}, " +
            //         $"stack trace: {ex.StackTrace}, " +
            //         $"inner exception message: {ex.InnerException?.InnerException?.Message}");

            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    fixations?.Clear();
            //}
        }

        private void DataReceivedCacheHandler(object sender, TcpListenerEventArgs args)
        {
            try
            {
                var xml = args.Data;
                var serializer = new XmlSerializer(typeof(root));
                root item = null;
                using (TextReader reader = new StringReader(xml))
                {
                    item = (root)serializer.Deserialize(reader);
                }

                // Save item to cache
                var timeArgs = item.TIME.Split('-');
                string timeString = $"{timeArgs[0]}:{timeArgs[1]}:{timeArgs[2]}";
                var fixationDateTime = DateTime.SpecifyKind(item.DATE.Date, DateTimeKind.Local);
                fixationDateTime = fixationDateTime.Add(TimeSpan.Parse(timeString));

                //TODO: Test
                _dataCacheProvider.SaveDataToFile<CameraFixationVM>(new List<CameraFixationVM>
                    {
                        new CameraFixationVM
                        {
                            PlateNumber = item.PLATE_STRING,
                            FixationDate = fixationDateTime
                        }
                    }, CacheDataType.CameraPlateCache);
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                    $"stack trace: {ex.StackTrace}, " +
                    $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                //MessageBox.Show(ex.Message);
            }
        }

        // Бинар принимает файл
        private void FileReceivedHandler(object sender, FileReceivedEventArgs args)
        {
            try
            {
                _logger.Info($"Start handling file: {args.FilePath}");

                var xmlParser = new ParserUtils();

                Thread.Sleep(1000);

                var fileContents = File.ReadAllText(args.FilePath);
                var xml = xmlParser.ParseXmlString(fileContents);

                string dateStr = xml.Story.StoryInfo.Date.Replace(' ', '-');
                string timeStr = $"{xml.Story.StoryInfo.Time.Hour}:{xml.Story.StoryInfo.Time.Minute}:{xml.Story.StoryInfo.Time.Second}";
                DateTime storyDate = Convert.ToDateTime(dateStr + " " + timeStr);

                var xmlConv = xml.Story.ImageList
                    .Where(x => Convert.ToInt32(x.ImageInfo.Speed) > 0)
                    .Select(x => new
                    {
                        x,
                        DateTime = Convert.ToDateTime(string.Format("{0} {1}",
                            x.ImageInfo.Date,
                            $"{x.ImageInfo.Time.Hour}:{x.ImageInfo.Time.Minute}:{x.ImageInfo.Time.Second}"))
                    }).ToList();

                var speedLimit = Convert.ToInt32(ConfigurationManager.AppSettings["SPEED_LIMIT"].ToString());
                var groupedIncidents = xmlConv.GroupBy(x => x.DateTime).ToList();

                var groupedAvg = groupedIncidents.Select(x => new
                {
                    x.Key,
                    Collection = x.ToList(),
                    Avg = x.Average(f => Convert.ToInt32(f.x.ImageInfo.Speed))
                })
                .Where(x => x.Avg > speedLimit)
                .OrderBy(x => x.Avg)
                .ToList();

                int index = Convert.ToInt32(groupedAvg.Count * 0.7) - 1; // - 1 because index in array starts from 0

                if (groupedAvg != null && groupedAvg.Any())
                {
                    var item = groupedAvg[index];
                    // Get camera data from cache
                    var cameraFixations = _dataCacheProvider.GetDataFromCache<CameraFixationVM>(CacheDataType.CameraPlateCache);
                    var dateFrom = storyDate.AddSeconds(-5);
                    var dateTo = storyDate.AddSeconds(5);

                    // Get fixation within 10 seconds
                    var filtered = cameraFixations?
                        .Where(x => x.FixationDate >= dateFrom && x.FixationDate < dateTo).ToList();

                    string plateNumber = string.Empty;
                    if (filtered != null && filtered.Any())
                    {
                        plateNumber = filtered.First().PlateNumber;
                    }
                    // Temp
                    //else
                    //{
                    //    plateNumber = "082MAZ02";
                    //}

                    // Show confirm popup (TODO: add timer)
                    Dispatcher.Invoke(() =>
                    {
                        if (!ManualInputPopup.IsOpen)
                        {
                            txtManualPlateNumber.Text = plateNumber;
                            _plateNumber = plateNumber;
                            _averageSpeed = (int)item.Avg;
                            _dateKey = item.Key;
                            _base64Image = item.Collection.First().x.Data;
                            _xmlFileName = args.FileName;
                            _xmlFilePath = args.FilePath;

                            ManualInputPopup.IsOpen = true;
                        }
                        else
                        {
                            //if (!string.IsNullOrWhiteSpace(plateNumber))
                            //{
                            //    _networkUtils.CheckConnection();

                            //    string base64 = item.Collection.First().x.Data;
                            //    int speed = (int)item.Avg;
                            //    var newFixation = new Fixation
                            //    {
                            //        BirthDate = DateTime.Today,
                            //        FixationDate = DateTime.SpecifyKind(item.Key, DateTimeKind.Local),
                            //        GRNZ = plateNumber,
                            //        FirstName = "Test",
                            //        LastName = "Test",
                            //        MiddleName = "Test",
                            //        PenaltySum = 0.0M,

                            //        // Additional
                            //        Image = base64,
                            //        Speed = speed
                            //    };

                            //    _logger.Info($"XML File parsed: {args.FileName}; {newFixation.Speed}; {newFixation.FixationDate}");

                            //    // Online mode
                            //    var sendResult = false;
                            //    if (NetworkUtils.NetworkConnectionIsAvailable)
                            //    {
                            //        // Save to DB
                            //        sendResult = await _httpClientProvider.SendFixationAsync(newFixation);
                            //    }

                            //    if (!NetworkUtils.NetworkConnectionIsAvailable || !sendResult)
                            //    {
                            //        // Save to cache
                            //        //_dataCacheProvider.SaveDataToFile<Fixation>(new List<Fixation> { newFixation }, CacheDataType.BinarCache);

                            //        // TODO: Temp + TODO: Display API STATUS with internet connection
                            //        _dataCacheProvider.SaveDataToFile<Fixation>(new List<Fixation> { newFixation }, CacheDataType.DatabaseCache);
                            //    }

                            //    // Clean camera fixations cache
                            //    //_dataCacheProvider.CleanCache(CacheDataType.CameraPlateCache);

                            //    // Delete temp file
                            //    //if (File.Exists(args.FilePath))
                            //    //{
                            //    //    File.Delete(args.FilePath);
                            //    //}                       

                            //    // Add image to UI
                            //    mainPage.Dispatcher.Invoke(() =>
                            //    {
                            //        _logger.Info($"Start dispatching to UI: {args.FilePath}");

                            //        BitmapImage bi = ConvertBase64ToImage(base64);
                            //        mainPage.imgMain.Source = bi;
                            //        mainPage.lblMainPlateNumber.Content = plateNumber;

                            //        //if (KeyWords.Any(x => description.ToLower().Contains(x))))
                            //        //{
                            //        //    mainPage.lblWarning.Content = "Внимание, ТС находится в угоне или розыске!";
                            //        //    mainPage.lblWarning.Background = Brushes.Red;
                            //        //}

                            //        // Add to ribbon
                            //        mainPage.listImagesRibbon.Items.Insert(0, new ListViewEventArgs
                            //        {
                            //            Image = bi,
                            //            PlateNumber = plateNumber
                            //        });

                            //        // Display speed
                            //        mainPage.lblSpeed.Content = speed.ToString();
                            //        // Display plate number
                            //        mainPage.lblPlateNumber.Content = plateNumber;
                            //    });
                            //}
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                    $"stack trace: {ex.StackTrace}, " +
                    $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                //MessageBox.Show(ex.Message);
            }
        }

        #endregion

        // TODO change
        private async void btnConfirmIncident_Click(object sender, RoutedEventArgs e)
        {
            btnConfirmIncident.IsEnabled = false;

            _networkUtils.CheckConnection();
            _plateNumber = txtManualPlateNumber.Text;
            string description = txtManualDescription.Text;

            if (!string.IsNullOrEmpty(_plateNumber))
            {
                string base64 = _base64Image;

                var iin = "000000000";
                List<Intruder> intruderList = await _httpClientProvider.GetIntrudersAsync($"$filter = IIN eq '{iin}");

                var intruder = intruderList.FirstOrDefault();

                


                if (intruder == null)
                {
                    intruder = new Intruder()
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        MiddleName = "Test",
                        BirthDate = DateTime.Today,                        
                        ImageBlobName = string.Empty,
                        ImageUrl = string.Empty
                    };
                }


                
                int speed = (int)_averageSpeed;
                var newFixation = new Fixation
                {
                    
                    FixationDate = DateTime.SpecifyKind(_dateKey, DateTimeKind.Local),
                    GRNZ = _plateNumber,
                    
                    PenaltySum = 0.0M,

                    Description = description,
                    PUNKT = txtManualStreet.Text,

                    
                    Speed = speed
                };

                _logger.Info($"XML File parsed: {_xmlFileName}; {newFixation.Speed}; {newFixation.FixationDate}");

                // Online mode
                var sendResult = false;
                if (NetworkUtils.NetworkConnectionIsAvailable)
                {
                    // Save to DB
                    sendResult = await _httpClientProvider.SendFixationAsync(newFixation);
                }

                if (!NetworkUtils.NetworkConnectionIsAvailable || !sendResult)
                {
                    // Save to cache
                    //_dataCacheProvider.SaveDataToFile<Fixation>(new List<Fixation> { newFixation }, CacheDataType.BinarCache);

                    // TODO: Temp + TODO: Display API STATUS with internet connection
                    _dataCacheProvider.SaveDataToFile<Fixation>(new List<Fixation> { newFixation }, CacheDataType.DatabaseCache);
                }

                // Clean camera fixations cache
                //_dataCacheProvider.CleanCache(CacheDataType.CameraPlateCache);

                // Delete temp file
                //if (File.Exists(args.FilePath))
                //{
                //    File.Delete(args.FilePath);
                //}                       

                // Add image to UI
                mainPage.Dispatcher.Invoke(() =>
                {
                    _logger.Info($"Start dispatching to UI: {_xmlFilePath}");

                    BitmapImage bi = ConvertBase64ToImage(base64);
                    mainPage.imgMain.Source = bi;
                    mainPage.lblMainPlateNumber.Content = _plateNumber;

                    var fixations = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.DatabaseCache);
                    bool inSearch = fixations
                           .Where(x => x.GRNZ == _plateNumber)
                           .Any(x => KeyWords.Any(y => !string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(y)));

                    mainPage.lblWarning.Content = inSearch ? "Внимание, ТС находится в розыске!" : string.Empty;
                    mainPage.lblWarning.Background = inSearch ? Brushes.Red : Brushes.LightGray;

                    //if (KeyWords.Any(x => description.ToLower().Contains(x)))
                    //{
                    //    mainPage.lblWarning.Content = "Внимание, ТС находится в розыске!";
                    //    mainPage.lblWarning.Background = Brushes.Red;
                    //}

                    // Add to ribbon
                    (mainPage.listImagesRibbon.ItemsSource as List<ListViewEventArgs>).Insert(0, new ListViewEventArgs
                    {
                        Image = bi,
                        PlateNumber = _plateNumber,
                        Description = description,
                        WarningColor = inSearch ? Brushes.Red : Brushes.LightGray, //KeyWords.Any(x => description.ToLower().Contains(x)) ? Brushes.Red : Brushes.LightGray,
                        WarningText = inSearch ? "ТС в розыске!" : string.Empty, //KeyWords.Any(x => description.ToLower().Contains(x)) ? "ТС в розыске!" : string.Empty,
                        Speed = speed,
                        FixationDate = newFixation.FixationDate
                    });

                    // Display speed
                    mainPage.lblSpeed.Content = $"{speed.ToString()} КМ/Ч";
                    // Display plate number
                    mainPage.lblPlateNumber.Content = _plateNumber;
                    // Display time
                    mainPage.lblTime.Content = newFixation.FixationDate.ToString("HH:mm:ss");
                    mainPage.lblDate.Content = newFixation.FixationDate.ToString("dd-MM-yyyy");
                });
            }
            btnConfirmIncident.IsEnabled = true;
            ManualInputPopup.IsOpen = false;
        }

        private void btnStartCounterMode_Click(object sender, RoutedEventArgs e)
        {
            isStarted = !isStarted;
            if (isStarted)
            {
                mainPage.btnStartCounterMode.Content = "Стоп";
                mainPage.btnStartCounterMode.Background = Brushes.LightCoral;
                mainPage.btnStartMainMode.IsEnabled = false;
                mainPage.btnStartNightMode.IsEnabled = false;

                var bw = new BackgroundWorker();
                bw.DoWork += (obj, args) =>
                {
                    try
                    {
                        // Find and display existed fixations
                        _tcpListener.DataReceived += DataReceivedDisplayHandler;
                        // Cache camera plate fixation
                        _tcpListener.DataReceived += DataReceivedCacheHandler;
                        // Binar main handler
                        _folderListener.FileReceived += FileReceivedHandler;

                        _folderListener.StartListener();
                        _tcpListener.StartListener();
                    }
                    catch (Exception ex)
                    {
                        _tcpListener.DataReceived -= DataReceivedDisplayHandler;
                        _tcpListener.DataReceived -= DataReceivedCacheHandler;
                        _folderListener.FileReceived -= FileReceivedHandler;

                        _folderListener.StopListener();
                        _tcpListener.StopListener();

                        _logger.Error($"Message: {ex.Message}, " +
                            $"stack trace: {ex.StackTrace}, " +
                            $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                        args.Result = $"{ex.Message}; {ex.InnerException?.InnerException?.Message}";
                    }
                };
                bw.RunWorkerCompleted += (obj, args) =>
                {
                    mainPage.btnStartCounterMode.Content = "Встречный режим";
                    mainPage.btnStartCounterMode.Background = Brushes.LightSlateGray;
                    mainPage.btnStartMainMode.IsEnabled = true;
                    mainPage.btnStartCounterMode.IsEnabled = true;
                    //isStarted = false;
                };
                bw.RunWorkerAsync();
            }
            else
            {
                mainPage.btnStartCounterMode.Content = "Встречный режим";
                mainPage.btnStartCounterMode.Background = Brushes.LightSlateGray;
                mainPage.btnStartMainMode.IsEnabled = true;
                mainPage.btnStartCounterMode.IsEnabled = true;

                _tcpListener.DataReceived -= DataReceivedDisplayHandler;
                _tcpListener.DataReceived -= DataReceivedCacheHandler;
                _folderListener.FileReceived -= FileReceivedHandler;

                _folderListener.StopListener();
                _tcpListener.StopListener();
            }
        }

        private void btnStartNightMode_Click(object sender, RoutedEventArgs e)
        {
            isStarted = !isStarted;
            if (isStarted)
            {
                mainPage.btnStartNightMode.Content = "Стоп";
                mainPage.btnStartNightMode.Background = Brushes.LightCoral;
                mainPage.btnStartMainMode.IsEnabled = false;
                mainPage.btnStartCounterMode.IsEnabled = false;

                var bw = new BackgroundWorker();
                bw.DoWork += (obj, args) =>
                {
                    try
                    {
                        // Find and display existed fixations
                        _tcpListener.DataReceived += DataReceivedDisplayHandler;
                        _tcpListener.StartListener();
                    }
                    catch (Exception ex)
                    {
                        _tcpListener.DataReceived -= DataReceivedDisplayHandler;
                        _tcpListener.StopListener();

                        _logger.Error($"Message: {ex.Message}, " +
                            $"stack trace: {ex.StackTrace}, " +
                            $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                        args.Result = $"{ex.Message}; {ex.InnerException?.InnerException?.Message}";
                    }
                };
                bw.RunWorkerCompleted += (obj, args) =>
                {
                    mainPage.btnStartNightMode.Content = "Ночной режим";
                    mainPage.btnStartNightMode.Background = Brushes.LightSlateGray;
                    mainPage.btnStartMainMode.IsEnabled = true;
                    mainPage.btnStartCounterMode.IsEnabled = true;
                    //isStarted = false;

                    //if (args.Result != null)
                    //{
                    //    MessageBox.Show(args.Result.ToString());
                    //}
                };
                bw.RunWorkerAsync();
            }
            else
            {
                mainPage.btnStartNightMode.Content = "Ночной режим";
                mainPage.btnStartNightMode.Background = Brushes.LightSlateGray;
                mainPage.btnStartMainMode.IsEnabled = true;
                mainPage.btnStartCounterMode.IsEnabled = true;

                _tcpListener.DataReceived -= DataReceivedDisplayHandler;
                _tcpListener.StopListener();
            }
        }

        private void btnStartMainMode_Click(object sender, RoutedEventArgs e)
        {
            isStarted = !isStarted;
            if (isStarted)
            {
                mainPage.btnStartMainMode.Content = "Стоп";
                mainPage.btnStartMainMode.Background = Brushes.LightCoral;
                mainPage.btnStartNightMode.IsEnabled = false;
                mainPage.btnStartCounterMode.IsEnabled = false;

                var bw = new BackgroundWorker();
                bw.DoWork += (obj, args) =>
                {
                    try
                    {
                        // Find and display existed fixations
                        _tcpListener.DataReceived += DataReceivedDisplayHandler;
                        // Cache camera plate fixation
                        _tcpListener.DataReceived += DataReceivedCacheHandler;
                        // Binar main handler
                        _folderListener.FileReceived += FileReceivedHandler;

                        _folderListener.StartListener();
                        _tcpListener.StartListener();
                    }
                    catch (Exception ex)
                    {
                        _tcpListener.DataReceived -= DataReceivedDisplayHandler;
                        _tcpListener.DataReceived -= DataReceivedCacheHandler;
                        _folderListener.FileReceived -= FileReceivedHandler;

                        _folderListener.StopListener();
                        _tcpListener.StopListener();

                        _logger.Error($"Message: {ex.Message}, " +
                            $"stack trace: {ex.StackTrace}, " +
                            $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                        args.Result = $"{ex.Message}; {ex.InnerException?.InnerException?.Message}";
                    }
                };
                bw.RunWorkerCompleted += (obj, args) =>
                {
                    mainPage.btnStartMainMode.Content = "Патрулирование";
                    mainPage.btnStartMainMode.Background = Brushes.LightSlateGray;
                    mainPage.btnStartNightMode.IsEnabled = true;
                    mainPage.btnStartCounterMode.IsEnabled = true;
                    // isStarted = false;

                    //if (args.Result != null)
                    //{
                    //    MessageBox.Show(args.Result.ToString());
                    //}
                };
                bw.RunWorkerAsync();
            }
            else
            {
                mainPage.btnStartMainMode.Content = "Патрулирование";
                mainPage.btnStartMainMode.Background = Brushes.LightSlateGray;
                mainPage.btnStartNightMode.IsEnabled = true;
                mainPage.btnStartCounterMode.IsEnabled = true;

                _tcpListener.DataReceived -= DataReceivedDisplayHandler;
                _tcpListener.DataReceived -= DataReceivedCacheHandler;
                _folderListener.FileReceived -= FileReceivedHandler;

                _folderListener.StopListener();
                _tcpListener.StopListener();

                //wbSample.Navigate("http://195.93.152.93:8080/flu/StrobeMediaPlayback.swf");
                //meStreamer.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void btnHidePopup_Click(object sender, RoutedEventArgs e)
        {
            _dialogFixationItems.Clear();
            FixationsPopup.IsOpen = false;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void mainPage_Loaded(object sender, RoutedEventArgs e)
        {
            mainPage.mapControl.IsManipulationEnabled = true;
            mainPage.mapControl.MouseWheelZoomEnabled = true;
            mainPage.mapControl.ShowCenter = false;
            mainPage.mapControl.Zoom = 10;
            mainPage.mapControl.Position = new PointLatLng(43.238949, 76.889709);
            mainPage.mapControl.IsTextSearchEnabled = true;
            mainPage.mapControl.TouchEnabled = true;

            mainPage.mapControl.MapProvider = GMap.NET.MapProviders.YandexMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;


            //GMapOverlay markersOverlay = new GMapOverlay("markers");
            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(-25.966688, 32.580528),
            //  GMarkerGoogleType.green);
            //markersOverlay.Markers.Add(marker);
            //gmap.Overlays.Add(markersOverlay);

            // TODO: Add markers
            // TODO: Refactor MapRenderProvider
            // TODO: Subscribe and 

            var marker = new GMapMarker(new PointLatLng(43.238949, 76.889709));
            marker.ZIndex = 10;
            mainPage.mapControl.Markers.Add(marker);

            //wbSample.Navigate(@"http://195.93.152.93:8080/flu/StrobeMediaPlayback.swf");

            //meStreamer.Source = new Uri(@"http://80.127.144.74:8001/mjpg/video.mjpg?COUNTER", UriKind.RelativeOrAbsolute);
            //meStreamer.LoadedBehavior = MediaState.Manual;
            //meStreamer.UnloadedBehavior = MediaState.Manual;

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (TabItem item in (tcMain as TabControl).Items)
            //{
            //    if (item == (tcMain as TabControl).SelectedItem)
            //    {
            //        //item.Background = Brushes.Red;#2D2C32
            //        item.Foreground = Brushes.Black;
            //    }
            //    else
            //    {
            //        BrushConverter bc = new BrushConverter();
            //        Brush brush = (Brush)bc.ConvertFrom("#464D57");
            //        brush.Freeze();

            //        item.Foreground = Brushes.White;
            //        item.Background = brush;
            //    }

            //}
        }

        private async void btnFind_Click(object sender, RoutedEventArgs e)
        {
            searchPage.btnFind.IsEnabled = false;
            if (!string.IsNullOrWhiteSpace(searchPage.txtQuery.Text))
            {
                List<Fixation> fixations = null;
                try
                {
                    _networkUtils.CheckConnection();
                    fixations = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.DatabaseCache);
                    if (fixations == null || fixations.Count == 0)
                    {
                        if (NetworkUtils.NetworkConnectionIsAvailable)
                        {
                            string query = $"/Fixations?$filter=GRNZ eq '{searchPage.txtQuery.Text}'&$orderby=FixationDate desc";
                            fixations = await _httpClientProvider.GetFixationsAsync(query);
                        }
                        else
                        {
                            fixations = new List<Fixation>();
                        }
                    }
                    else
                    {
                        fixations = fixations.Where(x => x.GRNZ == searchPage.txtQuery.Text).ToList();
                    }
                    //TODO change
                    if (fixations.Count > 0 && !FixationsPopup.IsOpen)
                    {
                        _dialogFixationItems = fixations.Select(x => new DialogFixationVM
                        {
                            //FullName = $"{x.Intruder.LastName} {x.Intruder.FirstName} {x.Intruder.MiddleName}",
                            GRNZ = x.GRNZ ?? string.Empty,
                            FixationDate = x.FixationDate.ToString("dd.MM.yyyy HH:mm:ss"),
                            Description = x.Description ?? string.Empty,
                            Speed = x.Speed,
                            Image = ConvertBase64ToImage(string.Empty), //image
                            Place = x.PUNKT ?? string.Empty
                        }).ToList();
                        dgFixations.ItemsSource = _dialogFixationItems;

                        FixationsPopup.IsOpen = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Message: {ex.Message}, " +
                          $"stack trace: {ex.StackTrace}, " +
                          $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                    //MessageBox.Show(ex.Message);
                }
                finally
                {
                    searchPage.btnFind.IsEnabled = true;
                    fixations?.Clear();
                }
            }
        }

        private BitmapImage ConvertBase64ToImage(string base64)
        {
            byte[] binaryData;
            BitmapImage bi = new BitmapImage();
            try
            {
                binaryData = Convert.FromBase64String(base64);
            }
            catch (FormatException ex)
            {
                if (!base64.EndsWith("==") || !base64.EndsWith("="))
                {
                    base64 += "==";
                }
                binaryData = Convert.FromBase64String(base64);
            }

            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }

        private void txtSearchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = (sender as TextBox).Text.ToLower();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var result = _dialogFixationItems.Where(x => x.FullName.ToLower().Contains(query)
                    || x.Description.ToLower().Contains(query)
                    || x.FixationDate.Contains(query)
                    || x.GRNZ.ToLower().Contains(query)
                    || x.Place.ToLower().Contains(query)
                    || x.Speed.ToString().Contains(query)).ToList();

                dgFixations.ItemsSource = result;
            }
            else
            {
                dgFixations.ItemsSource = _dialogFixationItems;
            }
        }
    }
}
