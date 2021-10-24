using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils
{
    internal static class AesUtil
    {
        public static String AES_encrypt(String input, string key,string Iv,int keyLength)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = keyLength;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = mkey(key,keyLength);
            aes.IV = mkey(Iv,128);
            
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(input);
                    cs.Write(xXml, 0, xXml.Length);
                    cs.FlushFinalBlock();
                }

                xBuff = ms.ToArray();
            }

            return Convert.ToBase64String(xBuff,Base64FormattingOptions.None);
        }
        public static String AES_decrypt(String Input, string key,string Iv,int keyLength)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = keyLength;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = mkey(key,keyLength);
                aes.IV = mkey(Iv,128);
                
                var decrypt = aes.CreateDecryptor();
                byte[] encryptedStr = Convert.FromBase64String(Input);

                string Plain_Text;

                using (var ms = new MemoryStream(encryptedStr))
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            Plain_Text = reader.ReadToEnd();
                        }
                    }
                }

                return Plain_Text;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private static byte[] mkey(string skey,int keyLength)
        {
            int length = keyLength / 8;
            byte[] key = Encoding.UTF8.GetBytes(skey);
            byte[] k =  GenerateEmptyArray(length);
            for (int i = 0; i < key.Length; i++)
            {
                //k[i % 16] = (byte)(k[i % 16] ^ key[i]);
                k[i] = key[i];
                if(i == length-1)
                    break;
            }

            return k;
        }
        public static string PasswordFixer(string skey,int keyLength)
        {
            int length = keyLength / 8;
            byte[] key = Encoding.UTF8.GetBytes(skey);
            byte[] k = GenerateEmptyArray(length);
            for (int i = 0; i < key.Length; i++)
            {
                k[i] = key[i];
                if(i == length-1)
                    break;
            }

            return Encoding.UTF8.GetString(k);
        }
        public static string IvFixer(string skey,int keyLength)
        {
            int length = keyLength / 8;
            byte[] key = Encoding.UTF8.GetBytes(skey);
            byte[] k = GenerateEmptyArray(16);
            for (int i = length; i < key.Length; i++)
            {
                k[i-length] = key[i];
                if(i == (length+16 - 1))
                    break;
            }

            return Encoding.UTF8.GetString(k);
        }

        private static byte[] GenerateEmptyArray(int length)
        {
            List<byte> bytes = new List<byte>();
            for (int x = 0; x < length; x++)
            {
                bytes.Add(111);
            }

            return bytes.ToArray();
        }
        public static int GetKeyLength(EncKeyLength enc)
        {
            switch (enc)
            {
                case EncKeyLength.L128:
                    return 128;
                case EncKeyLength.L192:
                    return 192;
                default:
                    return 256;
            }
        }
    }
}