using QuantumLink.networking.protocol;

namespace Common.networking.protocol.requests
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

        public readonly AccountOperation Operation;
        public readonly string Payload;

        public AccountRequest(AccountOperation operation, string payload)
        {
            this.Operation = operation;
            this.Payload = payload;
        }
    }
}
