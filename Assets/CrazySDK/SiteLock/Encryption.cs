using System;
using System.Security.Cryptography;
using System.Text;

namespace CrazyGames
{
    public static class Encryption
    {
        private static readonly UnicodeEncoding Encoder = new UnicodeEncoding();

        public static bool Verify(string domain, string signature, string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            var dataToVerify = Encoding.UTF8.GetBytes(domain);
            var signatureData = Convert.FromBase64String(signature);
            return rsa.VerifyData(dataToVerify, signatureData, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
    }
}