using QuantumLink.networking.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Server.networking
{    
    public delegate void SessionDisposedHandler(Session session);

    public class Session : IDisposable
    {
        protected delegate void OpcodeHandler(InboundPacket inboundPacket, NetworkStream networkStream);

        protected readonly TcpClient client;
        protected readonly NetworkStream networkStream;

        protected readonly Thread listenThread;
        private volatile bool stopping;
        
        public bool Disposed { get; private set; }

        private readonly Dictionary<byte, OpcodeHandler> handlers;

        public SessionDisposedHandler SessionDisposed;

        public Session(TcpClient client, SessionDisposedHandler sessionDisposedHandler)
        {
            this.client = client;
            this.SessionDisposed = sessionDisposedHandler;
            this.networkStream = client.GetStream();
            this.listenThread = new Thread(new ThreadStart(listenLoop));
            this.listenThread.Start();
            this.stopping = false;
            this.Disposed = false;
        }

        private void listenLoop()
        {
            while (!this.stopping)
            {
                if (networkStream.DataAvailable)
                {
                    try
                    {
                        InboundPacket inboundPacket = new InboundPacket(networkStream);
                        handlers[inboundPacket.Opcode](inboundPacket, networkStream);
                    }
                    catch (IOException)
                    {
                        Dispose();
                    }
                }
            }
        }

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (this.Disposed)
                return;

            if (disposing)
            {
                this.stopping = true;
                this.networkStream.Close();
                this.client.Close();
                this.SessionDisposed(this);
            }
            this.Disposed = true;
        }
    }
}
