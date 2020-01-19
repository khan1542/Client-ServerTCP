using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    class Program
    {
        private static byte[] buffer = new byte[1024];
        private static byte[] bufferClient = new byte[1024];
        private static Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> clientSockets = new List<Socket>();

        static void Main(string[] args)
        {
            Console.Title = "Сервер";
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Запуск сервера");
            socketServer.Bind(new IPEndPoint(IPAddress.Any, 100));
            socketServer.Listen(6);
            socketServer.BeginAccept(new AsyncCallback(AcceptCallBack), null);

        }

        private static void AcceptCallBack(IAsyncResult AR)
        {
            Socket socket = socketServer.EndAccept(AR);
            clientSockets.Add(socket);
            Console.WriteLine("Подключение клиента:");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            socketServer.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;

            try
            {
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(buffer, dataBuf, received);
                string text = Encoding.ASCII.GetString(dataBuf);

                Console.WriteLine("Сообщение получено: " + text + " " + Convert.ToString(DateTime.Now.ToLongTimeString()));

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == 'C')
                    {
                        text = text.Remove(i - 1);
                        break;
                    }
                }
                int num = Convert.ToInt32(text) * 10;
                Thread.Sleep(6500);
                Console.WriteLine("Обработка с задержкой 6.5сек (умножение на 10): " + num);
                byte[] received_buffer = Encoding.ASCII.GetBytes(Convert.ToString(num));

                socket.BeginSend(received_buffer, 0, received_buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (SocketException)
            {
                Console.WriteLine("Клиент отключился");
            }
        }
        private static void SendCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
