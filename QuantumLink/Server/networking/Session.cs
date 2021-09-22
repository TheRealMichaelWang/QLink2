using QuantumLink.networking.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Server.networking
{
    public delegate void ClientDisconnectedEventHandler(Session session);

    public class Session : IDisposable
    {
        protected delegate void OpcodeHandler(InboundPacket inboundPacket);

        protected readonly TcpClient client;
        protected readonly NetworkStream networkStream;

        protected readonly Thread listenThread;
        private volatile bool stopping;
        
        public bool Disposed { get; private set; }

        protected readonly Dictionary<byte, OpcodeHandler> handlers;
        protected readonly Queue<OutboundPacket> toSend;

        public ClientDisconnectedEventHandler ClientDisconnected;

        public Session(TcpClient client, ClientDisconnectedEventHandler clientDisconnectedHandler)
        {
            this.client = client;
            this.ClientDisconnected = clientDisconnectedHandler;
            this.networkStream = client.GetStream();
            this.handlers = new Dictionary<byte, OpcodeHandler>();
            this.toSend = new Queue<OutboundPacket>();
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
                        handlers[inboundPacket.Opcode](inboundPacket);
                    }
                    catch (IOException)
                    {
                        Dispose();
                    }
                }
                while(!this.stopping && toSend.Count > 0)
                {
                    try
                    {
                        toSend.Dequeue().Send(networkStream);
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
                this.ClientDisconnected(this);
            }
            this.Disposed = true;
        }
    }
}
