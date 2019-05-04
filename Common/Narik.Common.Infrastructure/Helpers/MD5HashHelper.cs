using System;
using System.Security.Cryptography;
using System.Text;

namespace Narik.Common.Infrastructure.Helpers
{
        public class Md5HashHelper
        {
            public static string GetMd5Hash(string input)
            {
                // Create a new instance of the MD5CryptoServiceProvider object.
                MD5 md5Hasher = MD5.Create();

                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }

            // Verify a hash against a string.
            public static bool VerifyMd5Hash(string input, string hash)
            {
                // Hash the input.
                string hashOfInput = GetMd5Hash(input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //protected string GetMD5Hash(string input)
            //{
            //    byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            //    MD5Managed md5 = new MD5Managed();
            //    byte[] hash = md5.ComputeHash(bs);

            //    StringBuilder sb = new StringBuilder();
            //    foreach (byte b in bs)
            //    {
            //        sb.Append(b.ToString("x2").ToLower());
            //    }

            //    return sb.ToString();
            //}
        }
    
}