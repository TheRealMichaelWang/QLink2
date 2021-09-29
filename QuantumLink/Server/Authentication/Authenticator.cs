using QuantumLink.Common.Networking.Protocol.Requests;
using QuantumLink.Common.Networking.Protocol.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantumLink.Server.Networking;

namespace QuantumLink.Server.Authentication
{
    public sealed class Authenticator : IDisposable
    {
        public AccountDatabase AccountDatabase { get; }

        private Dictionary<Account, HandledSession> _sessionLookups;
        private Dictionary<Session, Account> _accountLookups;

        private HashSet<Account> _authenticatedAccounts;

        public Authenticator(AccountDatabase accountDatabase, Server.Networking.Server server)
        {
            this.AccountDatabase = accountDatabase;
            this._sessionLookups = new Dictionary<Account, HandledSession>();
            this._accountLookups = new Dictionary<Session, Account>();
            this._authenticatedAccounts = new HashSet<Account>();
            server.ClientDisconnencted = HandleClientDisconnect;
        }

        public Account FindAccount(HandledSession session) => this._accountLookups[session];

        private void HandleClientDisconnect(Session session)
        {
            if (_accountLookups.ContainsKey(session))
                Logout(_accountLookups[session]);
        }

        public void Logout(Account account)
        {
            _authenticatedAccounts.Remove(account);
            _accountLookups.Remove(_sessionLookups[account]);
            _sessionLookups.Remove(account);
        }

        public StatusResponse Authenticate(AuthRequest authRequest, HandledSession session)
        {
            Account account;
            if (authRequest.CreateAccount)
            {
                try
                {
                    account = AccountDatabase.RegisterAccount(authRequest.Username, authRequest.Password);
                }
                catch (InvalidOperationException)
                {
                    return new StatusResponse(1, "Username already taken");
                }
            }
            else
            {
                if (!AccountDatabase.HasAccount(authRequest.Username))
                    return new StatusResponse(1, "Unkown or Invalid Username");
                account = AccountDatabase[authRequest.Username];
                if (account.Password != authRequest.Password)
                    return new StatusResponse(2, "Incorrect Password");
                if (_sessionLookups.ContainsKey(account))
                    return new StatusResponse(3, "Account already in session");
                account.LastLogon = DateTime.Now;
            }
            _authenticatedAccounts.Add(account);
            _sessionLookups.Add(account, session);
            _accountLookups.Add(session, account);
            return new StatusResponse(0, "Succesfully logged in");
        }

        public void DeleteAccount(Account account)
        {
            AccountDatabase.DeleteAccount(account);
            if (_authenticatedAccounts.Remove(account))
            {
                Session session = _sessionLookups[account];
                _sessionLookups.Remove(account);
                _accountLookups.Remove(session);
            }
        }

        public void Dispose() => AccountDatabase.Save();
    }
}
