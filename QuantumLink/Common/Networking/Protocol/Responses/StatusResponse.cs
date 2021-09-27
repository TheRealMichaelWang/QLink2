using QuantumLink.networking.protocol;

namespace Common.networking.protocol.responses
{
    public sealed class StatusResponse
    {
        public sealed class StatusResponseEncoder : OutboundPacket
        {
            public StatusResponseEncoder(StatusResponse statusResponse) : base(0)
            {
                this.Writer.Write(statusResponse.StatusCode);
                this.Writer.Write(statusResponse.Message);
            }
        }

        public sealed class StatusResponseDecoder : InboundPacket
        {
            public StatusResponse DecodedStatusResponse { get; }

            public StatusResponseDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                DecodedStatusResponse = new StatusResponse(this.Reader.ReadByte(), this.Reader.ReadString());
            }
        }

        public byte   StatusCode { get; }
        public string Message    { get; }

        public StatusResponse(byte statusCode, string message)
        {
            this.StatusCode = statusCode;
            this.Message = message;
        }
    }
}
