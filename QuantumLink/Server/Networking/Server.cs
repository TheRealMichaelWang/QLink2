using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.networking
{
    public delegate Session ClientConnectedEventHandler(TcpClient client);

    public sealed class Server
    {
        private readonly TcpListener _tcpListener;
        private readonly List<Session> _sessions;
        private readonly Thread _connectionThread;

        private volatile bool _stopping;

        public bool Running { get; private set; }

        public ClientConnectedEventHandler    ClientConnected     { get; set; }
        public ClientDisconnectedEventHandler ClientDisconnencted { get; set; }

        public Server(int port)
        {
            _sessions = new List<Session>();
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _connectionThread = new Thread(ConnectionLoop);
            this.Running = false;
        }

        public void Start()
        {
            if (this.Running)
                throw new InvalidOperationException();
            this._stopping = false;
            _tcpListener.Start();
            _connectionThread.Start();
            this.Running = true;
        }

        public void Stop()
        {
            _stopping = true;
            while (_connectionThread.IsAlive) { }
            _tcpListener.Stop();
            this.Running = false;
        }

        private void HandleSessionDisposed(Session session)
        {
            _sessions.Remove(session);
            ClientDisconnencted?.Invoke(session);
        }

        private void ConnectionLoop()
        {
            while (!_stopping)
            {
                if (_tcpListener.Pending())
                {
                    TcpClient client = _tcpListener.AcceptTcpClient();
                    Session toadd = ClientConnected != null ? ClientConnected(client) : new Session(client, HandleSessionDisposed);
                    _sessions.Add(toadd);
                }
            }
        }
    }
}
