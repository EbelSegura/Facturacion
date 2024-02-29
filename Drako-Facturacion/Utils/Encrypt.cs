using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Drako_Facturacion.Utils
{
    public class Encrypt
    {
        public static string GetSHA1(string str)
        {
            SHA1 sha1 = SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] strem = null;
            StringBuilder sb = new StringBuilder();
            strem = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < strem.Length; i++) sb.AppendFormat("{0:x2}", strem[i]);
            return sb.ToString();
        }
    }
}
