using TcpClientExample.etTcp;

namespace TcpClientExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            AsyncTcp tcp = new AsyncTcp();
            tcp.Connect("192.168.123.97", 4001);

            while (true)
            {
                var t = Console.ReadLine();
                tcp.Send(t + "\r\n");
            }
        }
    }
}