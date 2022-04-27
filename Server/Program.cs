using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Messenger.MsgClient;

namespace Messenger.MsgServer {
    public enum ServerState : byte {
        Running = 0,
        Dead
    }

    public class Server : IDisposable {
        private List<Client> _clients;
        private uint _maximumClients;
        private ServerState _state;
        private Socket _serverSocket;

        public uint MaximumClients { get => _maximumClients; set => SetMaximumClients(value); }
        public ServerState State { get => _state; }

        public Server(uint maximumClients) {
            _clients = new List<Client>();
            SetMaximumClients(maximumClients);
            _state = ServerState.Dead;

            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 7930));
            _serverSocket.Listen();
            _serverSocket.Blocking = true;
        }
        public void Dispose() {
            Shutdown();
            _serverSocket.Close();
        }

        public async Task RunAsync() {
            if (_state == ServerState.Running) return;

            _state = ServerState.Running;
            await Task.Run(() => {
                while (_state == ServerState.Running) {
                    using (var socket = _serverSocket.Accept()) {
                        Console.WriteLine("Connection!");
                    }
                }
            });
        }

        public void Shutdown() {
            _serverSocket.Shutdown(SocketShutdown.Both);
            _state = ServerState.Dead;
        }

        public void SetMaximumClients(uint maximumClients) {
            _maximumClients = maximumClients >= _clients.Count ? maximumClients : (uint)_clients.Count;
            _clients.EnsureCapacity((int)_maximumClients);
        }
    }
}

class Program {
    static void Main(string[] args) {
        Stream stdOut = Console.OpenStandardOutput();
    }
}