using System.IO;
using System.Net.Sockets;
using System.Text;

namespace QuantumLink.networking.protocol
{
    public abstract class Packet
    {
        public readonly byte Opcode;
        protected readonly Stream Stream;

        public Packet(byte opcode, Stream stream)
        {
            this.Opcode = opcode;
            this.Stream = stream;
        }
    }

    public class OutboundPacket : Packet
    {
        protected BinaryWriter Writer { get; }

        public OutboundPacket(byte opcode) : base(opcode, new MemoryStream())
        {
            Writer = new BinaryWriter(this.Stream, Encoding.UTF8);
            Writer.Write(opcode);
        }

        public void Send(NetworkStream networkStream)
        {
            MemoryStream memoryStream = (MemoryStream)this.Stream;
            memoryStream.WriteTo(networkStream);
            memoryStream.Close();
        }
    }

    public class InboundPacket : Packet
    {
        protected BinaryReader Reader { get; }

        public InboundPacket(NetworkStream networkStream) : base((byte)networkStream.ReadByte(), networkStream)
        {
            Reader = new BinaryReader(networkStream, Encoding.UTF8);
        }

        public InboundPacket(InboundPacket thisPacket) : base(thisPacket.Opcode, thisPacket.Stream)
        {
            Reader = thisPacket.Reader;
        }
    }
}