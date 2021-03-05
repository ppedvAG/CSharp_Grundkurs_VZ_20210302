using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Lab16_Exercise2_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            StartServer();
        }

        static void StartServer()
        {
            IPAddress ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 11000);

            try
            {
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(endPoint);
                listener.Listen(10);

                Console.WriteLine("Warte auf eine Verbindunng..");
                Socket handler = listener.Accept();

                string text = "";
                byte[] bytes = null;
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRecieved = handler.Receive(bytes);
                    text += Encoding.UTF32.GetString(bytes, 0, bytesRecieved);

                    byte[] message = null;
                    switch (text)
                    {
                        case "zahl":
                            Console.WriteLine("Zahl angefordert..");
                            message = Encoding.UTF32.GetBytes("15421<EOF>");
                            handler.Send(message);
                            text = ""; 
                            break;

                        case "zeichen":
                            Console.WriteLine("Zeichen angefordert..");
                            message = Encoding.UTF32.GetBytes("A<EOF>");
                            handler.Send(message);
                            text = "";
                            break;

                        case "string":
                            Console.WriteLine("String angefordert..");
                            message = Encoding.UTF32.GetBytes("Hallo ich bin ein String!!<EOF>");
                            handler.Send(message);
                            text = "";
                            break;

                        default:
                            Console.WriteLine("unbekannt angefordert..");
                            message = Encoding.UTF32.GetBytes("<EOF>");
                            handler.Send(message);
                            text = "";
                            break;
                    }

                    if (text == "exit")
                    {
                        break;
                    }
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Server wurde geschlossen...");
            Console.ReadLine();
        }
    }
}
