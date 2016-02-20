using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            string strSig = System.Net.WebUtility.UrlEncode(strHash2Base64);

            string sAuth = string.Format("SharedAccessSignature sr={0:S}&sig={1:S}&se={2:S}&skn={3:S}",
                urlEncoded,
                strSig,
                expiry,
                policyName);

            return sAuth;
        }

        public static void PushToEventHub(
            string sbNamespace,
            string eventHubName,
            string policyName,
            string sasKey,
            TimeSpan duration,
            string content)
        {
            string url = string.Format("https://{}.servicebus.windows.net/{}/messages",
                      sbNamespace,
                      eventHubName);

            Uri uri = new Uri(url);

            string signature = GenerateSignature(
                policyName,
                sasKey,
                uri,
                duration);

            var req = WebRequest.Create(uri);

            req.Headers.Add("Authorization", signature);

            byte[] payload = System.Text.Encoding.UTF8.GetBytes(content);
            req.GetRequestStream().Write(payload, 0, payload.Length);
            

            //byte[] buffer = new byte[1024 * 64];
            //int iRead;


            //using (var output = req.GetRequestStream())
            //{
            //    while ((iRead = content.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        output.Write(buffer, 0, iRead);
            //    }
            //}
        }
    }
}
