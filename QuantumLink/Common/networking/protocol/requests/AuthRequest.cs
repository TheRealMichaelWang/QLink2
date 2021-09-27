using QuantumLink.networking.protocol;

namespace Common.networking.protocol.requests
{
    public sealed class AuthRequest
    {
        public sealed class AuthRequestEncoder : OutboundPacket
        {
            public AuthRequestEncoder(AuthRequest request) : base(0)
            {
                this.Writer.Write(request.Username);
                this.Writer.Write(request.Password);
                this.Writer.Write(request.CreateAccount);
            }
        }

        public sealed class AuthRequestDecoder : InboundPacket
        {
            public readonly AuthRequest DecodedAuthRequest;

            public AuthRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                this.DecodedAuthRequest = new AuthRequest(this.Reader.ReadString(), this.Reader.ReadString(), this.Reader.ReadBoolean());
            }
        }

        public readonly string Username;
        public readonly string Password;

        public readonly bool CreateAccount;

        public AuthRequest(string username, string password, bool createAccount)
        {
            this.Username = username;
            this.Password = password;
            this.CreateAccount = createAccount;
        }
    }
}
