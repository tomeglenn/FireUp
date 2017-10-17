using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FireUp.Config;
using FireUp.Extensions;

namespace FireUp.Network
{
    public class Server : IServer
    {
        public int Port { get; }
        public ServerConfiguration Config { get; }
        public bool Started { get; private set; }

        private readonly UdpClient connection;
        private readonly IList<UdpConnectedClient> clients = new List<UdpConnectedClient>();
        private Timer pingTimer;

        public Server(int port, ServerConfiguration config = null)
        {
            Port = port;
            Config = config ?? new ServerConfiguration();

            connection = new UdpClient(Port);
        }

        public void Start()
        {
            BeginReceive();
            StartPingTimer();
            OnServerStarted();

            Started = true;
        }

        public void Stop()
        {
            StopPingTimer();
            connection.Close();
            OnServerStopped();

            Started = false;
        }
        
        public void Send(string message, params UdpConnectedClient[] recipients)
        {
            if (!Started)
            {
                Console.WriteLine("The server must be started before sending a message");
                return;
            }

            var data = message.ToUtf8Bytes();
            foreach (var client in recipients)
            {
                connection.Send(data, data.Length, client.Endpoint);
            }
        }

        public void Broadcast(string message)
        {
            Send(message, clients.ToArray());
        }

        private void StartPingTimer()
        {
            pingTimer = new Timer(OnPingInterval, null, TimeSpan.FromSeconds(Config.PingInterval), TimeSpan.FromSeconds(Config.PingInterval));
        }

        private void StopPingTimer()
        {
            pingTimer.Dispose();
        }

        private void OnPingInterval(object state)
        {
            var timedOutClients = clients.Where(x => x.HasTimedOut(Config.ClientTimeout)).ToList();
            foreach (var client in timedOutClients)
            {
                DisconnectClient(client);
            }

            Broadcast(ProtocolConstants.ServerPing);
        }

        private void BeginReceive()
        {
            connection.BeginReceive(OnReceive, null);
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint endpoint = null;

                var data = connection.EndReceive(asyncResult, ref endpoint);
                var message = data.ToUtf8String();

                var client = AddOrGetClient(endpoint);

                HandleMessage(client, message);
                BeginReceive();
            }
            catch (SocketException)
            {
                BeginReceive();
            }
        }

        private UdpConnectedClient AddOrGetClient(IPEndPoint endpoint)
        {
            var existingClient = clients.FirstOrDefault(x => Equals(x.Endpoint, endpoint));
            if (existingClient != null)
            {
                return existingClient;
            }

            var newClient = new UdpConnectedClient(endpoint);
            clients.Add(newClient);

            return newClient;
        }

        private void DisconnectClient(UdpConnectedClient client)
        {
            if (clients.Contains(client))
            {
                clients.Remove(client);
            }

            Send(ProtocolConstants.ServerDisconnect, client);
            OnClientDisconnected(client);
        }

        private void HandleMessage(UdpConnectedClient client, string message)
        {
            client.KeepAlive();

            switch (message)
            {
                case ProtocolConstants.ClientHandshake:
                    Send(ProtocolConstants.ServerAcknowledgeHandshake, client);
                    OnClientConnected(client);
                    break;
                case ProtocolConstants.ClientDisconnect:
                    clients.Remove(client);
                    OnClientDisconnected(client);
                    break;
                case ProtocolConstants.ClientPong:
                    OnClientPongReceived(client);
                    break;
                default:
                    OnClientMessageReceived(client, message);
                    break;
            }
        }

        protected virtual void OnServerStarted()
        {
            Console.WriteLine($"Server started listening on port {Port}");
        }

        protected virtual void OnServerStopped()
        {
            Console.WriteLine("Server stopped");
        }

        protected virtual void OnClientConnected(UdpConnectedClient client)
        {
            Console.WriteLine($"Client Connected: {client}");
        }

        protected virtual void OnClientDisconnected(UdpConnectedClient client)
        {
            Console.WriteLine($"Client Disconnected: {client}");
        }

        protected virtual void OnClientMessageReceived(UdpConnectedClient client, string message)
        {
            Console.WriteLine($"Client Messaged: {client}");
            Console.WriteLine($"{message}");
        }

        protected virtual void OnClientPongReceived(UdpConnectedClient client)
        {
            Console.WriteLine($"Client Pong Received: {client}");
        }
    }
}
