using QuantumLink.networking.protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.networking
{
    public sealed class Client : IDisposable
    {
        // Variables
        private readonly TcpClient client;
        private readonly IPEndPoint ipEndPoint;

        private readonly NetworkStream networkStream;
        private readonly BinaryWriter writer;
        private readonly BinaryReader reader;

        private bool disposed;

        // Setup client config
        public Client(IPAddress serverAddress, int serverPort)
        {
            this.ipEndPoint = new IPEndPoint(serverAddress, serverPort);
            this.client = new TcpClient(ipEndPoint);
            this.networkStream = client.GetStream();
            this.writer = new BinaryWriter(this.networkStream);
            this.reader = new BinaryReader(this.networkStream);
            this.disposed = false;
        }

        // Handle with incomming packets
        public InboundPacket MakeRequest(OutboundPacket request)
        {
            request.Send(this.networkStream);
            InboundPacket response = new InboundPacket(this.networkStream);
            return response;
        }

        //Stops processes
        public void Dispose()
        {
            if (this.disposed)
                return;
            this.networkStream.Close();
            this.client.Close();
            this.disposed = true;
        }
    }
}
