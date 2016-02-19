using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLXEtoEventHub.EventHub
{
    public class Publisher
    {

        public static string GenerateSignature(
            string policyName,
            string sasKey,
            Uri uri,
            TimeSpan duration)
        {
            string urlEncoded = System.Net.WebUtility.UrlEncode(uri.ToString());
            string expiry = DateTime.Now.Add(duration).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            string strToSign = string.Format("{0:S}\n{1:S}", urlEncoded, expiry);
            var bytesToSign = System.Text.Encoding.UTF8.GetBytes(strToSign);

            System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(sasKey));
            string strHash2Base64 = Convert.ToBase64String(SHA256.ComputeHash(bytesToSign));

            string sAuth = string.Format("SharedAccessSignature sr={0:S}&sig={1:S}&se={2:S}&skn={3:S}",
                urlEncoded,
                strHash2Base64,
                expiry,
                policyName);

            return sAuth;
        }
    }
}
