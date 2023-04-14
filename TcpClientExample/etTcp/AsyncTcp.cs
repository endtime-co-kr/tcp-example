using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static TcpClientExample.etTcp.AsyncTcp;

namespace TcpClientExample.etTcp
{
    public class AsyncTcp
    {
        private ManualResetEvent _connEvt = new ManualResetEvent(false);
        private bool _isConnected = false;
        private TcpClient _tcpClient = new TcpClient();
        private byte[] _recvBuffer= new byte[256];

        public bool Connect(string host, int port)
        {
            IPAddress.TryParse(host, out IPAddress? ipAddress);
            if (ipAddress == null)
            {
                return false;
            }

            _tcpClient = new TcpClient();
            _tcpClient.BeginConnect(host, port, new AsyncCallback(ConnectCallback), _tcpClient);
            _connEvt.WaitOne(5000);
            if (_isConnected)
            {
                Task.Run(async () => {
                    while (true)
                    {
                        await ReadLinesAsync(_tcpClient);
                    }
                    });
                return true;
            }
            return false;
        }

        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;
                if (client == null)
                {
                    return;
                }
                client.EndConnect(ar);
                Console.WriteLine("connected to {0}",
                    client.Client.RemoteEndPoint?.ToString());
                _isConnected = true;
                _connEvt.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }



        public async Task<string> ReadLinesAsync(TcpClient client)
        {            
            int readResult = await _tcpClient.GetStream().ReadAsync(_recvBuffer, 0, _recvBuffer.Length);
            Array.Resize(ref _recvBuffer, readResult);
            string recvString = Encoding.ASCII.GetString(_recvBuffer);
            Console.Write(recvString);
            return recvString;
        }


        public void Send(string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            _tcpClient.Client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), _tcpClient);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;
                int bytesSent = client.Client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
