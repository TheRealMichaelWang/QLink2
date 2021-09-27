using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumLink.Server.Authentication
{
    public sealed class AccountDatabase
    {
        public static AccountDatabase FromFile(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fileStream);
            int accounts = reader.ReadInt32();
            List<Account> accountList = new List<Account>(accounts);
            for (int i = 0; i < accounts; i++)
                accountList[i] = Account.FromStream(fileStream);
            AccountDatabase accountDatabase = new AccountDatabase(accountList, filePath);
            fileStream.Close();
            return accountDatabase;
        }

        public Account this[string username]
        {
            get =>_usernameLookup[username];
            set
            {
                if (!_usernameLookup.ContainsKey(username))
                    throw new InvalidOperationException("No such user \"" + username + "\" exists.");
            }
        }

        private string _saveFileSource;

        private List<Account> _accountList;
        private Dictionary<string, Account> _usernameLookup;

        public bool HasAccount(string username) => _usernameLookup.ContainsKey(username);

        private AccountDatabase(List<Account> accountList, string saveFileSource)
        {
            this._accountList = accountList;
            this._saveFileSource = saveFileSource;
            this._usernameLookup = new Dictionary<string, Account>();
            foreach (Account account in accountList)
            {
                if (_usernameLookup.ContainsKey(account.Username))
                    throw new InvalidOperationException("Accounts cannot have the same username.");
                _usernameLookup.Add(account.Username, account);
            }
        }

        public Account RegisterAccount(string username, string password)
        {
            if (_usernameLookup.ContainsKey(username))
                throw new InvalidOperationException("Cannot create an account with a username already taken.");
            Account account = new Account(username, password, DateTime.Now, DateTime.Now);
            this._accountList.Add(account);
            this._usernameLookup.Add(username, account);
            return account;
        }

        public void DeleteAccount(Account account)
        {
            if (!_usernameLookup.ContainsKey(account.Username))
                throw new ArgumentException("Account is not part of database.", "account");
            this._usernameLookup.Remove(account.Username);
            this._accountList.Remove(account);
        }

        public void Save()
        {
            if (_saveFileSource == null)
                throw new InvalidOperationException("Cannot save a database with no assigned file source.");
            FileStream fileStream = new FileStream(_saveFileSource, FileMode.Open, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(fileStream);
            writer.Write(_accountList.Count);
            foreach (Account account in _accountList)
                account.WriteToStream(fileStream);
            fileStream.Close();
        }
    }
}
