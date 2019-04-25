using Library;
using System;
using System.Net.Sockets;
using System.Threading;

namespace VolumeRocker
{
    internal class App
    {
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
                _tcpClient.Connect(Settings.HOST, Settings.PORT);
                NetworkStream stream = _tcpClient.GetStream();
                new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;
                        VolumeWatcher volumeWatcher = new VolumeWatcher();
                        int i = 0;
                        while (true)
                        {
                            if (++i == 100)
                            {
                                GC.Collect();
                                i = 0;
                            }
                            Thread.Sleep(10);
                            byte volume = Volume;
                            if (volume == 102)
                            {
                                continue;
                            }

                            if (volumeWatcher.Get() != volume)
                            {
                                volumeWatcher.Set(volume);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("closed connection?");
                    }
                }).Start();
                while (true)
                {
                    int currentVolume = stream.ReadByte();
                    if (currentVolume == -1)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    Volume = (byte)currentVolume;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }
    }
}