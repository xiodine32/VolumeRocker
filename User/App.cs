using Library;
using System;
using System.Net.Sockets;
using System.Threading;

namespace VolumeRocker
{
    internal class App
    {
        private const int PORT = 8123;
        private readonly TcpClient _tcpClient;
        private static readonly object _lock = new object();
        private static byte _volume = 102;
        private static byte Volume
        {
            get
            {
                lock (_lock)
                {
                    return _volume;
                }
            }
            set
            {
                lock (_lock)
                {
                    _volume = value;
                    Console.WriteLine("Volume: " + _volume);
                }
            }
        }

        public App()
        {
            _tcpClient = new TcpClient();
        }

        internal void Run()
        {
            try 
            { 
                _tcpClient.Connect("ROM-L37556", PORT);
                var stream = _tcpClient.GetStream();
                new Thread(() =>
                {
                    try { 
                    Thread.CurrentThread.IsBackground = true;
                    var volumeWatcher = new VolumeWatcher();
                    var i = 0;
                    while (true)
                    {
                        if (++i == 100)
                        {
                            GC.Collect();
                            i = 0;
                        }
                        Thread.Sleep(10);
                        var volume = Volume;
                        if (volume == 102) continue;
                        if (volumeWatcher.Get() != volume)
                        {
                            volumeWatcher.Set(volume);
                        }
                    }
                    } catch (Exception)
                    {
                        Console.WriteLine("closed connection?");
                    }
                }).Start();
                while (true)
                {
                    var currentVolume = stream.ReadByte();
                    if (currentVolume == -1)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    
                    Volume = (byte)currentVolume;
                }
            } catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }
    }
}