using System;
using System.Text;
using System.IO;

using System.Security.Cryptography;
using System.Xml.Serialization;

namespace practicaWin
{
    class RSA
    {
        private static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
        private RSAParameters privateKey;
        private RSAParameters publicKey;

        public RSA()
        {
            privateKey = rsa.ExportParameters(false);
            publicKey = rsa.ExportParameters(true);
        }

        public string GetPublicKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));

            xs.Serialize(sw, publicKey);

            return sw.ToString();
        }

        public string Encrypt(string plainText)
        {
            rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(privateKey);

            var data = Encoding.Unicode.GetBytes(plainText);
            var cypher = rsa.Encrypt(data, false);

            return Convert.ToBase64String(cypher);
        }

        public string Decrypt(string cypherText)
        {
            var dataBytes = Convert.FromBase64String(cypherText);

            rsa.ImportParameters(publicKey);

            var plainText = rsa.Decrypt(dataBytes, false);

            return Encoding.Unicode.GetString(plainText);
        }
    }
}
