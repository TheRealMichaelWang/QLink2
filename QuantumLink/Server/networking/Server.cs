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
        private readonly TcpListener tcpListener;
        private readonly List<Session> sessions;
        private readonly Thread connectionThread;

        private volatile bool stopping;

        public bool Running { get; private set; }

        public ClientConnectedEventHandler ClientConnected;

        public Server(int port)
        {
            sessions = new List<Session>();
            tcpListener = new TcpListener(IPAddress.Any, port);
            connectionThread = new Thread(new ThreadStart(connectionLoop));
            this.Running = false;
        }

        public void Start()
        {
            if (this.Running)
                throw new InvalidOperationException();
            this.stopping = false;
            tcpListener.Start();
            connectionThread.Start();
            this.Running = true;
        }

        public void Stop()
        {
            stopping = true;
            while (connectionThread.IsAlive) { }
            tcpListener.Stop();
            this.Running = false;
        }

        private void handleSessionDisposed(Session session)
        {
            sessions.Remove(session);
        }

        private void connectionLoop()
        {
            while (!stopping)
            {
                if (tcpListener.Pending())
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Session toadd;
                    if (ClientConnected != null)
                        toadd = ClientConnected(client);
                    else
                        toadd = new Session(client, handleSessionDisposed);
                    sessions.Add(toadd);
                }
            }
        }
    }
}
