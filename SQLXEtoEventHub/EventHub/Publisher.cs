using System;
using System.Net;

namespace SQLXEtoEventHub.EventHub
{
    public class Publisher
    {
        public static DateTime DT_START = new DateTime(1970, 1, 1);

        public static string GenerateSignature(
            string policyName,
            string sasKey,
            Uri uri,
            TimeSpan duration)
        {
            string urlEncoded = System.Net.WebUtility.UrlEncode(uri.ToString());

            DateTime dtEpiry = DateTime.Now.Add(duration);
            string expiry = ((int)(dtEpiry - DT_START).TotalSeconds).ToString();

            string strToSign = string.Format("{0:S}\n{1:S}", urlEncoded, expiry);
            var bytesToSign = System.Text.Encoding.UTF8.GetBytes(strToSign);

            System.Security.Cryptography.HMACSHA256 SHA256 = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(sasKey));
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
            byte[] payload)
        {
            string url = string.Format("https://{0:S}.servicebus.windows.net/{1:S}/messages",
                      sbNamespace,
                      eventHubName);

            Uri uri = new Uri(url);

            string signature = GenerateSignature(
                policyName,
                sasKey,
                uri,
                duration);

            var req = WebRequest.Create(uri);
            req.Method = "POST";
            req.ContentLength = payload.Length;

            req.Headers.Add("Authorization", signature);

            req.GetRequestStream().Write(payload, 0, payload.Length);

            var resp = req.GetResponse();
        }

        public static void PushToEventHub(
            string sbNamespace,
            string eventHubName,
            string policyName,
            string sasKey,
            TimeSpan duration,
            string content)
        {

            byte[] payload = System.Text.Encoding.UTF8.GetBytes(content);
            PushToEventHub(sbNamespace, eventHubName, policyName, sasKey, duration, payload);
        }
    }
}
