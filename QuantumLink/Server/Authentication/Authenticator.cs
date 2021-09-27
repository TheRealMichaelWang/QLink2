using Common.networking.protocol.requests;
using Common.networking.protocol.responses;
using Server.networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.authentication
{
    public sealed class Authenticator : IDisposable
    {
        public readonly AccountDatabase accountDatabase;

        private Dictionary<Account, HandledSession> sessionLookups;
        private Dictionary<Session, Account> accountLookups;

        private HashSet<Account> authenticatedAccounts;

        public Authenticator(AccountDatabase accountDatabase, Server.networking.Server server)
        {
            this.accountDatabase = accountDatabase;
            this.sessionLookups = new Dictionary<Account, HandledSession>();
            this.accountLookups = new Dictionary<Session, Account>();
            this.authenticatedAccounts = new HashSet<Account>();
            server.ClientDisconencted = handleClientDisconnect;
        }

        public Account FindAccount(HandledSession session) => this.accountLookups[session];

        private void handleClientDisconnect(Session session)
        {
            if (accountLookups.ContainsKey(session))
                Logout(accountLookups[session]);
        }

        public void Logout(Account account)
        {
            authenticatedAccounts.Remove(account);
            accountLookups.Remove(sessionLookups[account]);
            sessionLookups.Remove(account);
        }

        public StatusResponse Authenticate(AuthRequest authRequest, HandledSession session)
        {
            Account account;
            if (authRequest.CreateAccount)
            {
                try
                {
                    account = accountDatabase.RegisterAccount(authRequest.Username, authRequest.Password);
                }
                catch (InvalidOperationException)
                {
                    return new StatusResponse(1, "Username already taken");
                }
            }
            else
            {
                if (!accountDatabase.HasAccount(authRequest.Username))
                    return new StatusResponse(1, "Unkown or Invalid Username");
                account = accountDatabase[authRequest.Username];
                if (account.Password != authRequest.Password)
                    return new StatusResponse(2, "Incorrect Password");
                if (sessionLookups.ContainsKey(account))
                    return new StatusResponse(3, "Account already in session");
                account.LastLogon = DateTime.Now;
            }
            authenticatedAccounts.Add(account);
            sessionLookups.Add(account, session);
            accountLookups.Add(session, account);
            return new StatusResponse(0, "Succesfully logged in");
        }

        public void DeleteAccount(Account account)
        {
            accountDatabase.DeleteAccount(account);
            if (authenticatedAccounts.Remove(account))
            {
                Session session = sessionLookups[account];
                sessionLookups.Remove(account);
                accountLookups.Remove(session);
            }
        }

        public void Dispose() => accountDatabase.Save();
    }
}
