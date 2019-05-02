using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BinarApp.Providers
{
    /// <summary>
    /// Слушатель папок
    /// </summary>
    public class FolderListenerProvider : IFolderListenerProvider, IDisposable
    {
        public bool IsActive { get; set; }
        public event EventHandler<FileReceivedEventArgs> FileReceived;

        private FileSystemWatcher _fileSystemWatcher;
        private string _observableFolderPath;
        private string _cacheDataFolderPath;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public FolderListenerProvider()
        {
            _observableFolderPath = ConfigurationManager.AppSettings["InitialDataSourcePath"].ToString();
            _cacheDataFolderPath = ConfigurationManager.AppSettings["ProcessedDataSourcePath"].ToString();

            if (!Directory.Exists(_observableFolderPath))
            {
                Directory.CreateDirectory(_observableFolderPath);
            }

            if (!Directory.Exists(_cacheDataFolderPath))
            {
                Directory.CreateDirectory(_cacheDataFolderPath);
            }

            // Default
            //_observableFolderPath = @"C:\ProgramData\Simicon\grabberd";
            //_cacheDataFolderPath = @"C:\ProgramData\Simicon\grabberd\cache";

            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            _fileSystemWatcher.Path = _observableFolderPath;
            _fileSystemWatcher.Created += OnCreated;

            _logger.Info($"File watcher initialized, watching directory: {_observableFolderPath}");
        }

        public void Receive()
        {
            // Start listening
        }

        public void StartListener()
        {
            //_fileSystemWatcher.Created -= OnCreated;
            //_fileSystemWatcher.Error += OnError;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopListener()
        {
            //IsActive = false;
            //_fileSystemWatcher.Created -= OnCreated;
            //_fileSystemWatcher.Error -= OnError;
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        private void OnError(object source, ErrorEventArgs e)
        {
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                //  This can happen if Windows is reporting many file system events quickly 
                //  and internal buffer of the  FileSystemWatcher is not large enough to handle this
                //  rate of events. The InternalBufferOverflowException error informs the application
                //  that some of the file system events are being lost.
                //Console.WriteLine(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));

                _logger.Error($"Message: {e.GetException().Message}, " +
                                 $"stack trace: {e.GetException().StackTrace}, " +
                                 $"inner exception message: {e.GetException().InnerException?.InnerException?.Message}");
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                // Cache data
                _logger.Info($"File has been created on device: {e.Name}, full path: {e.FullPath}");
                if (!Directory.Exists(_cacheDataFolderPath))
                {
                    Directory.CreateDirectory(_cacheDataFolderPath);
                }

               // var newPath = Path.Combine(_cacheDataFolderPath, $"{e.Name}");
                //Thread.Sleep(100);
                //File.Copy(e.FullPath, newPath);

                //if (File.Exists(newPath))
                //{
                //    _logger.Info($"File has been cached on device: {e.Name}, full cache path: {newPath}");
                //}

                // Raise file created event
                OnFileReceive(e.Name, e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"Message: {ex.Message}, " +
                             $"stack trace: {ex.StackTrace}, " +
                             $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                //throw ex;
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
        }
    }
}
