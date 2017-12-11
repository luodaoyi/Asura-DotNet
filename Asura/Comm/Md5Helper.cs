using System;
using System.Security.Cryptography;
using System.Text;

namespace Asura.Comm
{
    public static class Md5Helper
    {
        public static string Md5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}