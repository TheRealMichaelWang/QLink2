using QuantumLink.networking.protocol;

namespace Common.networking.protocol.requests
{
    public sealed class AuthRequest
    {
        public sealed class AuthRequestEncoder : OutboundPacket
        {
            public AuthRequestEncoder(AuthRequest request) : base(0)
            {
                this.writer.Write(request.Username);
                this.writer.Write(request.Password);
            }
        }

        public sealed class AuthRequestDecoder : InboundPacket
        {
            public readonly AuthRequest DecodedAuthRequest;

            public AuthRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                this.DecodedAuthRequest = new AuthRequest(this.reader.ReadString(), this.reader.ReadString());
            }
        }

        public readonly string Username;
        public readonly string Password;

        public AuthRequest(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}
