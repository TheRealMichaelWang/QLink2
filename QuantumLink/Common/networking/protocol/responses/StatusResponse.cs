﻿using QuantumLink.networking.protocol;

namespace Common.networking.protocol.responses
{
    public sealed class StatusResponse
    {
        public sealed class StatusResponseEncoder : OutboundPacket
        {
            public StatusResponseEncoder(StatusResponse statusResponse) : base(0)
            {
                this.writer.Write(statusResponse.StatusCode);
                this.writer.Write(statusResponse.Message);
            }
        }

        public sealed class StatusResponseDecoder : InboundPacket
        {
            public readonly StatusResponse DecodedStatusResponse;

            public StatusResponseDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                DecodedStatusResponse = new StatusResponse(this.reader.ReadByte(), this.reader.ReadString());
            }
        }

        public readonly byte StatusCode;
        public readonly string Message;

        public StatusResponse(byte statusCode, string message)
        {
            this.StatusCode = statusCode;
            this.Message = message;
        }
    }
}
