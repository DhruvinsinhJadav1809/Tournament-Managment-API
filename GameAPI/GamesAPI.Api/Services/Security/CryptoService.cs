using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace GamesAPI.Api.Services.Security
{
    public class CryptoService : ICryptoService
    {
        private readonly byte[] _key;

        public CryptoService(
            IConfiguration configuration)
        {
            var key = configuration["Encryption:Key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception(
                    "Encryption key is missing.");
            }

            if (key.Length != 32)
            {
                throw new Exception(
                    "Encryption key must be exactly 32 characters.");
            }

            _key = Encoding.UTF8.GetBytes(key);
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();

            aes.Key = _key;

            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor();

            using var memoryStream = new MemoryStream();

            using var cryptoStream = new CryptoStream(
                memoryStream,
                encryptor,
                CryptoStreamMode.Write);

            using var writer = new StreamWriter(cryptoStream);

            writer.Write(plainText);

            writer.Flush();

            cryptoStream.FlushFinalBlock();

            var cipherText =
                Convert.ToBase64String(memoryStream.ToArray());

            var iv =
                Convert.ToBase64String(aes.IV);

            return $"{iv}:{cipherText}";
        }

        public string Decrypt(string encryptedText)
        {
            var parts = encryptedText.Split(':');

            var iv = Convert.FromBase64String(parts[0]);

            var cipherBytes = Convert.FromBase64String(parts[1]);

            using var aes = Aes.Create();

            aes.Key = _key;

            aes.IV = iv;

            var decryptor = aes.CreateDecryptor();

            using var memoryStream =
                new MemoryStream(cipherBytes);

            using var cryptoStream =
                new CryptoStream(
                    memoryStream,
                    decryptor,
                    CryptoStreamMode.Read);

            using var reader =
                new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
    }
}