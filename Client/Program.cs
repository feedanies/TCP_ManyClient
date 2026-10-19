using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client...");

            var ip = IPAddress.Parse("192.168.0.131");
            var port = 27001;
            var ep = new IPEndPoint(ip, port);

            var client = new TcpClient();

            try
            {
                client.Connect(ep);
                if (client.Connected)
                {
                    Console.WriteLine("Connected...");

                    //Server-e melumat gonderir

                    var writer = Task.Run(() =>
                    {
                        while (true)
                        {
                            var text = Console.ReadLine();
                            var stream = client.GetStream();
                            var bw = new BinaryWriter(stream);
                            bw.Write(text ?? "NoMessage");
                        }
                    });


                    //Server-den melumat qebul edib, cap edir

                    var reader = Task.Run(() =>
                    {
                        while (true)
                        {
                            var text = Console.ReadLine();
                            var stream = client.GetStream();
                            var br = new BinaryReader(stream);
                            var result = br.ReadString();
                            Console.WriteLine($"From server: {result}");
                        }
                    });

                    Task.WaitAll(writer, reader);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
