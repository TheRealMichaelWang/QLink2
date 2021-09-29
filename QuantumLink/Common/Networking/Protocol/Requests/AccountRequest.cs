namespace QuantumLink.Common.Networking.Protocol.Requests
{
    public sealed class AccountRequest
    {
        public sealed class AccountRequestEncoder : OutboundPacket
        {
            public AccountRequestEncoder(AccountRequest accountRequest) : base(1)
            {
                this.Writer.Write((byte)accountRequest.Operation);
                this.Writer.Write(accountRequest.Payload);
            }
        }

        public sealed class AccountRequestDecoder : InboundPacket
        {
            public readonly AccountRequest DecodedAccountRequest;

            public AccountRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                DecodedAccountRequest = new AccountRequest((AccountOperation)this.Reader.ReadByte(), this.Reader.ReadString());
            }
        }

        public enum AccountOperation
        {
            Logout,
            ChangePassword,
            DeleteAccount
        }

        public AccountOperation Operation { get; }
        public string Payload { get; }

        public AccountRequest(AccountOperation operation, string payload)
        {
            this.Operation = operation;
            this.Payload = payload;
        }
    }
}
