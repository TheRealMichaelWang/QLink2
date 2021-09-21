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
        // Encode the request for a request from an account
        public sealed class AccountRequestEncoder : OutboundPacket
        {
            public AccountRequestEncoder(AccountRequest accountRequest) : base(1)
            {
                this.writer.Write((byte)accountRequest.Operation);
            }
        }

        // Decode the request for a request from an account
        public sealed class AccountRequestDecoder : InboundPacket
        {
            public readonly AccountRequest DecodedAccountRequest;

            public AccountRequestDecoder(InboundPacket inboundPacket) : base(inboundPacket)
            {
                DecodedAccountRequest = new AccountRequest((AccountOperation)this.reader.ReadByte());
            }
        }

        // Operations that can happen to an account
        public enum AccountOperation
        {
            Logout,
            QueryInfo,
            DeleteAccount
        }

        public readonly AccountOperation Operation;

        // Request an action to happen on an account
        public AccountRequest(AccountOperation operation)
        {
            this.Operation = operation;
        }
    }
}
