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
            handlers.Add(1, handleAccountRequest);
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
            try
            {
                Account sessionAccount = Authenticator.FindAccount(this);
                string statusMsg;
                if (operation == AccountRequest.AccountOperation.Logout)
                {
                    Authenticator.Logout(sessionAccount);
                    statusMsg = "Succesfully logged out";
                }
                else if (operation == AccountRequest.AccountOperation.ChangePassword)
                {
                    sessionAccount.Password = accountRequestDecoder.DecodedAccountRequest.Payload;
                    statusMsg = "Succesfully changed password";
                }
                else
                {
                    Authenticator.DeleteAccount(sessionAccount);
                    statusMsg = "Account succesfully deleted";
                }
                this.toSend.Enqueue(new StatusResponse.StatusResponseEncoder(new StatusResponse(0, statusMsg)));
            }
            catch (KeyNotFoundException)
            {
                this.toSend.Enqueue(new StatusResponse.StatusResponseEncoder(new StatusResponse(1, "You're not logged in")));
            }
        }
    }
}
