namespace QuantumLink.Common.Networking.Protocol.Requests
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
            public AuthRequest DecodedAuthRequest { get; }

            public AuthRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                this.DecodedAuthRequest = new AuthRequest(this.Reader.ReadString(), this.Reader.ReadString(), this.Reader.ReadBoolean());
            }
        }

        public string Username { get; }
        public string Password { get; }
        public bool CreateAccount { get; }

        public AuthRequest(string username, string password, bool createAccount)
        {
            this.Username = username;
            this.Password = password;
            this.CreateAccount = createAccount;
        }
    }
}
