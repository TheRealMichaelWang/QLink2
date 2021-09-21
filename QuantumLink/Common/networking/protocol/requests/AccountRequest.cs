using QuantumLink.networking.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.networking.protocol.requests
{
    public sealed class AccountRequest
    {
        public sealed class AccountRequestEncoder : OutboundPacket
        {
            public AccountRequestEncoder(AccountRequest accountRequest) : base(1)
            {
                this.writer.Write((byte)accountRequest.Operation);
            }
        }

        public sealed class AccountRequestDecoder : InboundPacket
        {
            public readonly AccountRequest DecodedAccountRequest;

            public AccountRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                DecodedAccountRequest = new AccountRequest((AccountOperation)this.reader.ReadByte());
            }
        }

        public enum AccountOperation
        {
            Logout,
            QueryInfo,
            DeleteAccount
        }

        public readonly AccountOperation Operation;

        public AccountRequest(AccountOperation operation)
        {
            this.Operation = operation;
        }
    }
}
