﻿using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace BrickApp.Services
{
    public class PageUserServices : IUserServices
    {
        private readonly IConfiguration _config;

        public PageUserServices(IConfiguration config)
        {
            _config = config;
        }

        public bool ValidateUser(string username, string password)
        {
            return username == _config["user:username"] && VerifyHashedPassword(password, _config);
        }

        private bool VerifyHashedPassword(string password, IConfiguration config)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(config["user:salt"]);

            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            );

            string hashText = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return hashText == config["user:password"];
        }
    }
}
