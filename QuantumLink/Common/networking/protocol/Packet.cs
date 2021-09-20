using System.IO;
using System.Net.Sockets;
using System.Text;

namespace QuantumLink.networking.protocol
{
    public class Packet
    {
        public readonly byte Opcode;
        protected readonly Stream stream;

        public Packet(byte opcode, Stream stream)
        {
            this.Opcode = opcode;
            this.stream = stream;
        }
    }

    public class OutboundPacket : Packet
    {
        protected readonly BinaryWriter writer;

        public OutboundPacket(byte opcode) : base(opcode, new MemoryStream())
        {
            writer = new BinaryWriter(this.stream, Encoding.UTF8);
            writer.Write(opcode);
        }

        public void Send(NetworkStream networkStream)
        {
            MemoryStream memoryStream = (MemoryStream)this.stream;
            memoryStream.WriteTo(networkStream);
            memoryStream.Close();
        }
    }

    public class InboundPacket : Packet
    {
        protected readonly BinaryReader reader;

        public InboundPacket(NetworkStream networkStream) : base((byte)networkStream.ReadByte(), networkStream)
        {
            reader = new BinaryReader(networkStream, Encoding.UTF8);
        }
    }
}
