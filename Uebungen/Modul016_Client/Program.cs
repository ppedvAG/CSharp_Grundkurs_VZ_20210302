using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab16_Exercise2_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            StarteClient();
        }

        static void StarteClient()
        {
            IPAddress ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 11000);
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(endPoint);
                Console.WriteLine($"Verbunden mit Endpoint: {sender.RemoteEndPoint.ToString()}");

                while (true)
                {
                    string text = Console.ReadLine();
                    byte[] message = Encoding.UTF32.GetBytes(text);
                    int bytesSend = sender.Send(message);
                    Console.WriteLine($"gesendete Bytes: {bytesSend}");

                    byte[] bytes = null; 
                    string response = ""; 
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRecieved = sender.Receive(bytes);
                        response += Encoding.UTF32.GetString(bytes, 0, bytesRecieved);

                        if (response.Contains("<EOF>"))
                        {
                            Console.WriteLine(response);
                            break; 
                        }
                    }

                    if (text == "exit")
                    {
                        break;
                    }
                }

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
