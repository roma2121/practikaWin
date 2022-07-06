using System;
using System.Text;

using System.Security.Cryptography;

namespace practicaWin
{
    class AES
    {
        AesCryptoServiceProvider aes;

        public AES()
        {
            aes = new AesCryptoServiceProvider();

            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.GenerateIV();
            aes.GenerateKey();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            objRsa = new RSA();
        }

        RSA objRsa;

        public byte[] Key(byte[] pubKey)
        {
            byte[] key = (byte[])aes.Key;
            string s = "";
            for (int i = 0; i < key.Length; i++)
            {
                s += key[i].ToString();
            }
            return objRsa.Encrypt((key), pubKey);
        }

        public string Encrypt(string clearText)
        {
            byte[] kkey = aes.Key;

            ICryptoTransform transform = aes.CreateEncryptor();

            byte[] encryptedBytes = transform.TransformFinalBlock(ASCIIEncoding.ASCII.GetBytes(clearText), 0, clearText.Length);

            string str = Convert.ToBase64String(encryptedBytes);

            return str;
        }

        public string Decrypt(string encryptedText)
        {
            ICryptoTransform transform = aes.CreateDecryptor();

            byte[] encBytes = Convert.FromBase64String(encryptedText);
            byte[] decBytes = transform.TransformFinalBlock(encBytes, 0, encBytes.Length);

            string str = ASCIIEncoding.ASCII.GetString(decBytes);
            return str;
        }
    }
}
