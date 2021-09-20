using System.IO;
using System.Net.Sockets;
using System.Text;

namespace QuantumLink.networking.protocol
{
    // Default packet
    public class Packet
    {
        public readonly byte Opcode;
        protected readonly Stream stream;

        // Setup packet config
        public Packet(byte opcode, Stream stream)
        {
            this.Opcode = opcode;
            this.stream = stream;
        }
    }

    public class OutboundPacket : Packet
    {
        protected readonly BinaryWriter writer;

        // Set outbound packet info
        public OutboundPacket(byte opcode) : base(opcode, new MemoryStream())
        {
            writer = new BinaryWriter(this.stream, Encoding.UTF8);
            writer.Write(opcode);
        }

        // Send the packet
        public void Send(NetworkStream networkStream)
        {
            MemoryStream memoryStream = (MemoryStream)this.stream;
            memoryStream.WriteTo(networkStream);
            memoryStream.Close();
        }
    }

    // Packet being recieved
    public class InboundPacket : Packet
    {
        protected readonly BinaryReader reader;

        // Reads info from incomming packet
        public InboundPacket(NetworkStream networkStream) : base((byte)networkStream.ReadByte(), networkStream)
        {
            reader = new BinaryReader(networkStream, Encoding.UTF8);
        }
    }
}
