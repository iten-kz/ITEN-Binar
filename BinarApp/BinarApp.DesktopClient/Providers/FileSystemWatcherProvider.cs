using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using NLog;
using System;
using System.Configuration;
using System.IO;

namespace BinarApp.DesktopClient.Providers
{
    public class FileSystemWatcherProvider : IFolderListenerProvider, IDisposable
    {
        public event EventHandler<FileReceivedEventArgs> FileReceived;

        private FileSystemWatcher _fileSystemWatcher;
        private string _observableFolderPath;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public FileSystemWatcherProvider()
        {
            _observableFolderPath = ConfigurationManager.AppSettings["TATTILE_FILE_PATH"].ToString();

            if (!Directory.Exists(_observableFolderPath))
            {
                Directory.CreateDirectory(_observableFolderPath);
            }

            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string todayFolderPath = Path.Combine(_observableFolderPath, today);
            if (!Directory.Exists(todayFolderPath))
            {
                Directory.CreateDirectory(todayFolderPath);
            }

            //string currHour = DateTime.Now.Hour.ToString();

            for (int i = 0; i < 24; i++)
            {
                var hour = i.ToString();
                if (hour.Length == 1)
                {
                    hour = "0" + hour;
                }

                string hourFolderPath = Path.Combine(todayFolderPath, hour);
                if (!Directory.Exists(hourFolderPath))
                {
                    Directory.CreateDirectory(hourFolderPath);
                }
            }

            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            _fileSystemWatcher.Path = _observableFolderPath;
            _fileSystemWatcher.IncludeSubdirectories = true;
            _fileSystemWatcher.Created += OnCreated;

            _logger.Info($"Binar file watcher initialized, watching directory: {_observableFolderPath}");
        }

        public void StartListener()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopListener()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                _logger.Info($"File has been created on device: {e.Name}, full path: {e.FullPath}");

                // Raise file created event
                OnFileReceive(e.Name, e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                             $"stack trace: {ex.StackTrace}, " +
                             $"inner exception message: {ex.InnerException?.InnerException?.Message}");
            }
        }

        private void OnFileReceive(string fileName, string filePath)
        {
            FileReceived?.Invoke(this, new FileReceivedEventArgs
            {
                FileName = fileName,
                FilePath = filePath
            });
        }

        public void Dispose()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            _fileSystemWatcher.Dispose();
        }
    }
}
