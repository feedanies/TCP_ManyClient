using System.Net;
using System.Net.Sockets;

namespace TCP_Lesson2
{
    internal class Program
    {
        static TcpListener listener = null;
        static BinaryWriter bw = null;
        static BinaryReader br = null;

        static List<TcpClient> clients = new List<TcpClient>(); //clientlarin hamisini listde saxlayib gonderilen mesaji hamisina gondermek ucun


        static void Main(string[] args)
        {

            Console.WriteLine("Server...");
            var ip = IPAddress.Parse("192.168.0.131");
            var port = 27001;
            var ep = new IPEndPoint(ip, port);

            listener = new TcpListener(ep);
            listener.Start();

            Console.WriteLine($"Listening: {listener.LocalEndpoint}");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                clients.Add(client); //butun clientlar bura elave edilir
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");


                Task.Run(() =>
                {
                    //Server butun clientlara mesaj gonderir
                    var writer = Task.Run(() =>
                    {
                        while (true)
                        {
                            var message = Console.ReadLine();
                            foreach (var c in clients)
                            {
                                var stream = c.GetStream();
                                bw = new BinaryWriter(stream);
                                bw.Write(message ?? "NoMessage");
                            }
                        }
                    });

                    //Server butun clientlardan melumat qebul edib, cap edecek

                    while (true)
                    {
                        foreach (var c in clients)
                        {
                            var stream = c.GetStream();
                            if (stream.DataAvailable) //eger streamde data varsa, onu oxuyacaq, yoxdursa, skip edecek
                            {
                                br = new BinaryReader(stream);
                                var message = br.ReadString();
                                Console.WriteLine($"Message from {c.Client.RemoteEndPoint}: {message}");
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                });

            }



        }
    }
}
