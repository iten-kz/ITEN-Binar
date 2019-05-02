using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarApp.Providers
{
    public class TcpListenerProvider : ITcpListenerProvider, IDisposable
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private TcpListener _server = null;
        private TcpClient _client = null;

        private string _ip = "127.0.0.1"; // 127.0.0.1 : 192.168.103
        private int _port = 7800;
        private IPAddress _localAddr;

        //private CancellationTokenSource _tokenSource;

        public bool IsActive { get; set; }
        public event EventHandler<TcpListenerEventArgs> DataReceived;

        public TcpListenerProvider()
        {
            //_ip = ConfigurationManager.AppSettings["Ip"].ToString();
            //_port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
            _localAddr = IPAddress.Parse(_ip);
            _server = new TcpListener(_localAddr, _port);
        }

        public void StartListener()
        {
            //_tokenSource = new CancellationTokenSource();
            //var token = _tokenSource.Token;

            //var tcpTask = new Task(() => Receive(token), token);


            //var tcpTask = new Task(() => Receive());
            //tcpTask.ContinueWith((Task task) =>
            //{
            //    if (task.Status == TaskStatus.Faulted)
            //    {
            //        tcpTask.Dispose();
            //        throw task.Exception;

            //        //StartListener();
            //    }
            //}, TaskContinuationOptions.OnlyOnFaulted);

            //tcpTask.Start();        

            IsActive = true;
            Receive();
        }

        private void Receive()//(CancellationToken token)
        {
            // Start listening for client requests.
            try
            {
                _server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                string data = null;

                // Enter the listening loop.
                while (IsActive)
                {
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    _client = _server.AcceptTcpClient();

                    //if (IsActive)
                    //{
                    data = null;
                    // Get a stream object for reading and writing
                    NetworkStream stream = _client.GetStream();
                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);

                        // Raise data received event
                        OnDataReceive(data);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                if (ex is SocketException && (ex as SocketException).ErrorCode == 10004)
                {
                    _logger.Info($"TcpListener stopped");
                }
                else
                {
                    _logger.Error($"Message: {ex.Message}, " +
                         $"stack trace: {ex.StackTrace}, " +
                         $"inner exception message: {ex.InnerException?.InnerException?.Message}");

                    throw ex;
                }
            }
            finally
            {
                // Shutdown and end connection
                _client?.Close();
                _server?.Stop();
                //_tokenSource.Dispose();
            }
        }

        public void StopListener()
        {
            IsActive = false;

            //_client?.Close();
            //_server?.Stop();

            //_tokenSource.Cancel();
        }

        private void OnDataReceive(string data)
        {
            DataReceived?.Invoke(this, new TcpListenerEventArgs
            {
                Data = data
            });
        }

        public void Dispose()
        {
            StopListener();
            //_client?.Close();
            //_server?.Stop();
        }
    }
}
