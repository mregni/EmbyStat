using System.Security.Cryptography;

namespace Emby.ApiClient.Cryptography
{
    public class CryptographyProvider : ICryptographyProvider
    {
        public byte[] CreateSha1(byte[] value)
        {
            using (var provider = SHA1.Create())
            {
                return provider.ComputeHash(value);
            }
        }

        public byte[] CreateMD5(byte[] value)
        {
            using (var provider = MD5.Create())
            {
                return provider.ComputeHash(value);
            }
        }
    }
}
