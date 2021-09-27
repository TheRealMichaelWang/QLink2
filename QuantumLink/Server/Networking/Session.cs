using QuantumLink.Common.Networking.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace QuantumLink.Server.Networking
{
    public delegate void ClientDisconnectedEventHandler(Session session);

    public class Session : IDisposable
    {
        protected delegate void OpcodeHandler(InboundPacket inboundPacket);

        protected readonly TcpClient Client;
        protected readonly NetworkStream NetworkStream;

        protected readonly Thread ListenThread;
        private volatile bool _stopping;
        
        public bool Disposed { get; private set; }

        protected readonly Dictionary<byte, OpcodeHandler> Handlers;
        protected readonly Queue<OutboundPacket> ToSend;

        public ClientDisconnectedEventHandler ClientDisconnected;

        public Session(TcpClient client, ClientDisconnectedEventHandler clientDisconnectedHandler)
        {
            this.Client = client;
            this.ClientDisconnected = clientDisconnectedHandler;
            this.NetworkStream = client.GetStream();
            this.Handlers = new Dictionary<byte, OpcodeHandler>();
            this.ToSend = new Queue<OutboundPacket>();
            this.ListenThread = new Thread(ListenLoop);
            this.ListenThread.Start();
            this._stopping = false;
            this.Disposed = false;
        }

        private void ListenLoop()
        {
            while (!this._stopping)
            {
                if (NetworkStream.DataAvailable)
                {
                    try
                    {
                        InboundPacket inboundPacket = new InboundPacket(NetworkStream);
                        Handlers[inboundPacket.Opcode](inboundPacket);
                    }
                    catch (IOException)
                    {
                        Dispose();
                    }
                }
                while(!this._stopping && ToSend.Count > 0)
                {
                    try
                    {
                        ToSend.Dequeue().Send(NetworkStream);
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
                this._stopping = true;
                this.NetworkStream.Close();
                this.Client.Close();
                this.ClientDisconnected(this);
            }
            this.Disposed = true;
        }
    }
}
