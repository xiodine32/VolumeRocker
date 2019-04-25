using Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Admin
{
    class Program
    {
        private static readonly object _lock = new object();
        private static byte _volume = 100;
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
                }
            }
        }
        static void ClientThread(TcpClient client)
        {
            byte currentVolume = 0;
            var stream = client.GetStream();
            while (true)
            {
                try { 
                Thread.Sleep(100);
                var volume = Volume;
                if (volume == currentVolume) continue;
                currentVolume = volume;
                Console.WriteLine("* " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() + " sending: " + currentVolume);
                stream.WriteByte(currentVolume);
                } catch (Exception)
                {
                    Console.WriteLine("* " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() + " client closed!");
                    return;
                }
            }
        }
        static void Main()
        {
            Console.WriteLine("Starting server!");
            var listener = new TcpListener(IPAddress.Any, 8123);
            Console.WriteLine("Server ready!");
            listener.Start();
            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                var volumeWatcher = new VolumeWatcher();
                volumeWatcher.Change += (a) => Volume = a;
                volumeWatcher.Watch();
            }).Start();
            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine("new client! " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    ClientThread(client);
                }).Start();
            }
        }
    }
}
