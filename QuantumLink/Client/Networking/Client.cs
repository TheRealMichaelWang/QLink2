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
    public sealed partial class Client : IDisposable
    {
        private readonly TcpClient _client;
        private readonly IPEndPoint _ipEndPoint;

        private readonly NetworkStream _networkStream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;

        private bool _disposed;

        public Client(IPAddress serverAddress, int serverPort)
        {
            this._ipEndPoint = new IPEndPoint(serverAddress, serverPort);
            this._client = new TcpClient(_ipEndPoint);
            this._networkStream = _client.GetStream();
            this._writer = new BinaryWriter(this._networkStream);
            this._reader = new BinaryReader(this._networkStream);
            this._disposed = false;
        }

        public InboundPacket MakeRequest(OutboundPacket request)
        {
            request.Send(this._networkStream);
            InboundPacket response = new InboundPacket(this._networkStream);
            return response;
        }

        public void Dispose()
        {
            if (this._disposed)
                return;
            this._networkStream.Close();
            this._client.Close();
            this._disposed = true;
        }
    }
}
