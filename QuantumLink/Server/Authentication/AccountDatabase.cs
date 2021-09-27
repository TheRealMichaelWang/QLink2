﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.authentication
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
            get
            {
                return usernameLookup[username];
            }
            set
            {
                if (!usernameLookup.ContainsKey(username))
                    throw new InvalidOperationException("No such user \"" + username + "\" exists.");
            }
        }

        private string saveFileSource;

        private List<Account> accountList;
        private Dictionary<string, Account> usernameLookup;

        public bool HasAccount(string username) => usernameLookup.ContainsKey(username);

        private AccountDatabase(List<Account> accountList, string saveFileSource)
        {
            this.accountList = accountList;
            this.saveFileSource = saveFileSource;
            this.usernameLookup = new Dictionary<string, Account>();
            foreach (Account account in accountList)
            {
                if (usernameLookup.ContainsKey(account.Username))
                    throw new InvalidOperationException("Accounts cannot have the same username.");
                usernameLookup.Add(account.Username, account);
            }
        }

        public Account RegisterAccount(string username, string password)
        {
            if (usernameLookup.ContainsKey(username))
                throw new InvalidOperationException("Cannot create an account with a username already taken.");
            Account account = new Account(username, password, DateTime.Now, DateTime.Now);
            this.accountList.Add(account);
            this.usernameLookup.Add(username, account);
            return account;
        }

        public void DeleteAccount(Account account)
        {
            if (!usernameLookup.ContainsKey(account.Username))
                throw new ArgumentException("Account is not part of database.", "account");
            this.usernameLookup.Remove(account.Username);
            this.accountList.Remove(account);
        }

        public void Save()
        {
            if (saveFileSource == null)
                throw new InvalidOperationException("Cannot save a database with no assigned file source.");
            FileStream fileStream = new FileStream(saveFileSource, FileMode.Open, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(fileStream);
            writer.Write(accountList.Count);
            foreach (Account account in accountList)
                account.WriteToStream(fileStream);
            fileStream.Close();
        }
    }
}