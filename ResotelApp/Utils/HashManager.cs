using System.Security.Cryptography;
using System.Text;
using static System.Text.Encoding;

namespace ResotelApp.Utils
{
    class HashManager
    {
        public static string SHA256(string entry)
        {
            if(entry == null)
            {
                return null;
            }
           SHA256Managed sha256 = new SHA256Managed();
           byte[] hash = sha256.ComputeHash(UTF8.GetBytes(entry));
           StringBuilder sb = new StringBuilder();
           for(int i=0; i<hash.Length;i++)
            {
                sb.AppendFormat("{0:X2}", hash[i]);
            }
            string result = sb.ToString();
            return result;
        }
    }
}
