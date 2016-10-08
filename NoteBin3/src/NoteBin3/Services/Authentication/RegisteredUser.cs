using LiteDB;
using NoteBin3.Types;
using System;
using System.Collections.Generic;

namespace NoteBin3.Services.Authentication
{
    public class RegisteredUser
    {
        public string Username { get; set; }

        public byte[] PasswordKey { get; set; }

        public byte[] CryptoSalt { get; set; }

        public PasswordCryptoConfiguration PasswordCryptoConf { get; set; }

        public Guid Identifier { get; set; }

        public List<Notebook> Notebooks { get; set; }

        [BsonId]
        public ObjectId DatabaseIdentifier { get; set; }
    }
}