using QuantumLink.networking.protocol;

namespace Common.networking.protocol.requests
{
    public sealed class AccountRequest
    {
        public sealed class AccountRequestEncoder : OutboundPacket
        {
            public AccountRequestEncoder(AccountRequest accountRequest) : base(1)
            {
                this.writer.Write((byte)accountRequest.Operation);
                this.writer.Write(accountRequest.Password);
            }
        }

        public sealed class AccountRequestDecoder : InboundPacket
        {
            public readonly AccountRequest DecodedAccountRequest;

            public AccountRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                DecodedAccountRequest = new AccountRequest((AccountOperation)this.reader.ReadByte(), this.reader.ReadString());
            }
        }

        public enum AccountOperation
        {
            Logout,
            ChangeUsername,
            DeleteAccount
        }

        public readonly AccountOperation Operation;
        public readonly string Password;

        public AccountRequest(AccountOperation operation, string password)
        {
            this.Operation = operation;
            this.Password = password;
        }
    }
}
