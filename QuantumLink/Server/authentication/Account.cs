using System;
using System.IO;

namespace Server.authentication
{
    public sealed class Account
    {
        public static Account FromStream(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            return new Account(reader.ReadString(), reader.ReadString(), new DateTime(reader.ReadInt64()), new DateTime(reader.ReadInt64()));
        }

        public readonly string Username;
        public readonly string Password;

        public DateTime LastLogon;
        public DateTime CakeDay;

        public Account(string username, string passsword, DateTime lastLoggon, DateTime cakeDay)
        {
            this.Username = username;
            this.Password = passsword;
            this.LastLogon = lastLoggon;
            this.CakeDay = cakeDay;
        }

        public void WriteToStream(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(this.Username);
            writer.Write(this.Password);
            writer.Write(this.LastLogon.Ticks);
            writer.Write(this.CakeDay.Ticks);
        }
    }
}
