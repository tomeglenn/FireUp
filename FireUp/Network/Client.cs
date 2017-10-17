using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FireUp.Config;
using FireUp.Extensions;

namespace FireUp.Network
{
    public class Client : IClient
    {
        public bool Connected { get; private set; }

        private readonly UdpClient connection;
        private IPEndPoint endpoint;

        public Client(string host, int port)
        {
            endpoint = new IPEndPoint(ParseHost(host), port);
            connection = new UdpClient();
        }

        public void Connect()
        {
            Send(ProtocolConstants.ClientHandshake);
            BeginReceive();
        }

        public void Disconnect()
        {
            Send(ProtocolConstants.ClientDisconnect);
            EndConnection();
        }
        
        public void Send(string message)
        {
            var data = message.ToUtf8Bytes();
            connection.Send(data, data.Length, endpoint);
        }

        private void BeginReceive()
        {
            try
            {
                connection.BeginReceive(OnReceive, null);
            }
            catch (SocketException exception)
            {
                Console.WriteLine(exception.Message);
                EndConnection();
            }
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                endpoint = null;
                var data = connection.EndReceive(asyncResult, ref endpoint);

                HandleMessage(data.ToUtf8String());
                BeginReceive();
            }
            catch (SocketException exception)
            {
                Console.WriteLine(exception.Message);
                EndConnection();
            }
        }

        private void HandleMessage(string message)
        {
            switch (message)
            {
                case ProtocolConstants.ServerAcknowledgeHandshake:
                    StartConnection();
                    break;
                case ProtocolConstants.ServerDisconnect:
                    EndConnection();
                    break;
                case ProtocolConstants.ServerPing:
                    Send(ProtocolConstants.ClientPong);
                    OnServerPingReceived();
                    break;
                default:
                    OnMessageReceived(message);
                    break;
            }
        }

        private void StartConnection()
        {
            Connected = true;
            OnConnect();
        }

        private void EndConnection()
        {
            Connected = false;
            OnDisconnect();
        }

        private static IPAddress ParseHost(string host)
        {
            var loopbackAddresses = new[] { "local", "localhost", "loopback" };
            if (loopbackAddresses.Any(x => x == host))
            {
                host = "127.0.0.1";
            }

            return IPAddress.Parse(host);
        }

        protected void OnConnect()
        {
            Console.WriteLine("Connected to server");
        }

        protected void OnDisconnect()
        {
            Console.WriteLine("Disconnected from server");
        }

        protected void OnMessageReceived(string message)
        {
            Console.WriteLine($"Message received: {message}");
        }

        protected void OnServerPingReceived()
        {
            Console.WriteLine("Server Ping Received");
        }
    }
}
