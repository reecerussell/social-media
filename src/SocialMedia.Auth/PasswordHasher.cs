using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SocialMedia.Core;
using System;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using SocialMedia.Core.Extensions;

namespace SocialMedia.Auth
{
    /// <summary>
    /// A deconstructed implementation of the PasswordHasher in Microsoft.AspNetCore.Identity. The aspects of
    /// Identity's V3 hashing has been taken and used here.
    /// https://github.com/dotnet/aspnetcore/blob/master/src/Identity/Extensions.Core/src/PasswordHasher.cs
    /// </summary>
    internal class PasswordHasher : IPasswordHasher
    {
        private readonly int _iterationCount;
        private readonly int _saltSize;
        private readonly int _bytesRequested;

        private readonly RandomNumberGenerator _rng;

        public PasswordHasher(IConfiguration configuration)
        {
            _rng = RandomNumberGenerator.Create();

            _iterationCount = configuration.GetInt(Constants.PasswordIterationCountKey, 15000);
            _saltSize = configuration.GetInt(Constants.PasswordSaltSizeKey, 128) / 8;
            _bytesRequested = configuration.GetInt(Constants.PasswordKeySizeKey, 256) / 8;
        }

        public string Hash(string password)
        {
            var salt = new byte[_saltSize];
            _rng.GetBytes(salt);
            var subKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, _iterationCount, _bytesRequested);

            var outputBytes = new byte[13 + salt.Length + subKey.Length];
            outputBytes[0] = 0x01; // format marker

            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA256);
            WriteNetworkByteOrder(outputBytes, 5, (uint)_iterationCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)128 / 8);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subKey, 0, outputBytes, 13 + salt.Length, subKey.Length);

            return Convert.ToBase64String(outputBytes);
        }


        public bool Verify(string hashedPassword, string password)
        {
            try
            {
                var hash = Convert.FromBase64String(hashedPassword);

                // Read header information
                var prf = (KeyDerivationPrf)ReadNetworkByteOrder(hash, 1);
                var iterationCount = (int)ReadNetworkByteOrder(hash, 5);
                var saltLength = (int)ReadNetworkByteOrder(hash, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < _saltSize)
                {
                    return false;
                }

                var salt = new byte[saltLength];
                Buffer.BlockCopy(hash, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                var subKeyLength = hash.Length - 13 - salt.Length;
                if (subKeyLength < 128 / 8)
                {
                    return false;
                }

                var expectedSubKey = new byte[subKeyLength];
                Buffer.BlockCopy(hash, 13 + salt.Length, expectedSubKey, 0, expectedSubKey.Length);

                // Hash the incoming password and verify it
                var actualSubKey = KeyDerivation.Pbkdf2(password, salt, prf, iterationCount, subKeyLength);

                return CryptographicOperations.FixedTimeEquals(actualSubKey, expectedSubKey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                   | ((uint)(buffer[offset + 1]) << 16)
                   | ((uint)(buffer[offset + 2]) << 8)
                   | ((uint)(buffer[offset + 3]));
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }
    }
}
