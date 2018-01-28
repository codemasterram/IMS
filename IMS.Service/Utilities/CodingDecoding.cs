using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;


namespace IMS.Logic
{
    public static class CodingDecoding
    {
        public static string ConvertToMD5Hash(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(s.Trim());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string ConvertToBase64String(this byte[] data)
        {
            if (data == null)
                return null;
            var base64 = Convert.ToBase64String(data);
            var src = String.Format("data:image;base64,{0}", base64);
            return src;
        }

        public static string EncodeToBase64(this string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;
            byte[] encData_byte = new byte[plainText.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(plainText);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }

        public static string DecodeFromBase64(this string encodedData)
        {
            if (string.IsNullOrEmpty(encodedData))
                return null;
            UTF8Encoding encoder = new UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
    }
}
