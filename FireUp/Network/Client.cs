using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FireUp.Config;
using FireUp.Extensions;

namespace FireUp.Network
{
    public class Client : IClient
    {
        public bool Connected { get; private set; }
        public ClientConfiguration Config { get; }

        private readonly UdpClient connection;
        private UdpConnectedClient server;
        private Timer timeoutTimer;

        public Client(string host, int port, ClientConfiguration config = null)
        {
            Config = config ?? new ClientConfiguration();

            server = new UdpConnectedClient(new IPEndPoint(ParseHost(host), port));
            connection = new UdpClient();
        }

        public void Connect()
        {
            Send(ProtocolConstants.ClientHandshake);
            BeginReceive();
            StartTimeoutTimer();
        }

        public void Disconnect()
        {
            StopTimeoutTimer();
            Send(ProtocolConstants.ClientDisconnect);
            EndConnection();
        }
        
        public void Send(string message)
        {
            var data = message.ToUtf8Bytes();
            connection.Send(data, data.Length, server.Endpoint);
        }

        private void StartTimeoutTimer()
        {
            timeoutTimer = new Timer(OnTimeoutInterval, null, TimeSpan.FromSeconds(Config.ServerTimeout), TimeSpan.FromSeconds(Config.ServerTimeout));
        }

        private void OnTimeoutInterval(object state)
        {
            if (server.HasTimedOut(Config.ServerTimeout))
            {
                Console.WriteLine("Connection with the server has timed out");
                Disconnect();
            }
        }

        private void StopTimeoutTimer()
        {
            timeoutTimer.Dispose();
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
                IPEndPoint endpoint = null;
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

        protected virtual void OnConnect()
        {
            Console.WriteLine("Connected to server");
        }

        protected virtual void OnDisconnect()
        {
            Console.WriteLine("Disconnected from server");
        }

        protected virtual void OnMessageReceived(string message)
        {
            Console.WriteLine($"Message received: {message}");
        }

        protected virtual void OnServerPingReceived()
        {
            Console.WriteLine("Server Ping Received");
        }
    }
}
