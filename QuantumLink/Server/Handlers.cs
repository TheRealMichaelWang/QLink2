using Common.networking.protocol.requests;
using Common.networking.protocol.responses;
using QuantumLink.networking.protocol;
using Server.authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.networking
{
    public sealed class HandledSession : Session
    {
        private volatile Authenticator Authenticator;

        public HandledSession(TcpClient client, ClientDisconnectedEventHandler sessionDisposedHandler, Authenticator authenticator) : base(client, sessionDisposedHandler)
        {
            handlers.Add(0, handleAuthRequest);
            this.Authenticator = authenticator;
        }

        private void handleAuthRequest(InboundPacket inboundPacket)
        {
            AuthRequest.AuthRequestDecoder authRequestDecoder = new AuthRequest.AuthRequestDecoder(inboundPacket);
            StatusResponse response = Authenticator.Authenticate(authRequestDecoder.DecodedAuthRequest, this);
            this.toSend.Enqueue(new StatusResponse.StatusResponseEncoder(response));
        }

        private void handleAccountRequest(InboundPacket inboundPacket)
        {
            AccountRequest.AccountRequestDecoder accountRequestDecoder = new AccountRequest.AccountRequestDecoder(inboundPacket);
            AccountRequest.AccountOperation operation = accountRequestDecoder.DecodedAccountRequest.Operation;
            if(operation == AccountRequest.AccountOperation.Logout)
            {

            }
            else if(operation == AccountRequest.AccountOperation.DeleteAccount)
            {

            }
        }
    }
}
