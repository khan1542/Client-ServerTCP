using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    class Program
    {
        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static int n;
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите номер клиента");
            n = int.Parse(Console.ReadLine());
            Console.Title = "Клиент " + n;
            Connect();
            Send();

            Console.ReadLine();

        }
        private static void Send()
        {
            try
            {
                Thread.Sleep(2900);
                while (true)
                {
                    var rand = new Random();
                    byte[] buffer = Encoding.ASCII.GetBytes(Convert.ToString(rand.Next(50, 76)) + " Client:" + n);
                    clientSocket.Send(buffer);

                    byte[] receivedBuf = new byte[1024];
                    int rec = clientSocket.Receive(receivedBuf);
                    byte[] data = new byte[rec];
                    Console.WriteLine("Отправка сообщения с задержкой 2.9 сек: ");
                    Array.Copy(receivedBuf, data, rec);
                    Console.WriteLine("Ответ от сервера:" + Encoding.ASCII.GetString(data));

                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Сервер отключился");
            }

        }

        private static void Connect()
        {

            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Loopback, 100);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Попытка подключения:" + attempts.ToString());
                }
            }
            Console.Clear();
            Console.WriteLine("Подключено");
        }
    }
}