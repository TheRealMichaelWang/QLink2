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

        public string Username { get; }
        public string Password { get; set; }

        public DateTime LastLogon { get; set; }
        public DateTime CakeDay   { get; }

        public Account(string username, string password, DateTime lastLogon, DateTime cakeDay)
        {
            this.Username = username;
            this.Password = password;
            this.LastLogon = lastLogon;
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
