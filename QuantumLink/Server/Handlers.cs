﻿using Common.networking.protocol.requests;
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
        private volatile Authenticator _authenticator;

        public HandledSession(TcpClient client, ClientDisconnectedEventHandler sessionDisposedHandler, Authenticator authenticator) : base(client, sessionDisposedHandler)
        {
            Handlers.Add(0, HandleAuthRequest);
            Handlers.Add(1, HandleAccountRequest);
            this._authenticator = authenticator;
        }

        private void HandleAuthRequest(InboundPacket inboundPacket)
        {
            AuthRequest.AuthRequestDecoder authRequestDecoder = new AuthRequest.AuthRequestDecoder(inboundPacket);
            StatusResponse response = _authenticator.Authenticate(authRequestDecoder.DecodedAuthRequest, this);
            this.ToSend.Enqueue(new StatusResponse.StatusResponseEncoder(response));
        }

        private void HandleAccountRequest(InboundPacket inboundPacket)
        {
            AccountRequest.AccountRequestDecoder accountRequestDecoder = new AccountRequest.AccountRequestDecoder(inboundPacket);
            AccountRequest.AccountOperation operation = accountRequestDecoder.DecodedAccountRequest.Operation;
            try
            {
                Account sessionAccount = _authenticator.FindAccount(this);
                string statusMsg;
                switch (operation)
                {
                    case AccountRequest.AccountOperation.Logout:
                        _authenticator.Logout(sessionAccount);
                        statusMsg = "Succesfully logged out";
                        break;
                    case AccountRequest.AccountOperation.ChangePassword:
                        sessionAccount.Password = accountRequestDecoder.DecodedAccountRequest.Payload;
                        statusMsg = "Succesfully changed password";
                        break;
                    default:
                        _authenticator.DeleteAccount(sessionAccount);
                        statusMsg = "Account succesfully deleted";
                        break;
                }
                this.ToSend.Enqueue(new StatusResponse.StatusResponseEncoder(new StatusResponse(0, statusMsg)));
            }
            catch (KeyNotFoundException)
            {
                this.ToSend.Enqueue(new StatusResponse.StatusResponseEncoder(new StatusResponse(1, "You're not logged in")));
            }
        }
    }
}
