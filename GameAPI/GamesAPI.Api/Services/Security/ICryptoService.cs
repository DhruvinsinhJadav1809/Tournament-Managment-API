namespace GamesAPI.Api.Services.Security
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);

        string Decrypt(string cipherText);
    }
}