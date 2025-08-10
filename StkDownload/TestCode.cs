using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public class TestCode
    {
        public static async Task TestProxy_WebshareAsync()
        {
            //============
            //config
            string proxyStr = "142.147.128.93:6593:lagengct:x1m482lfqwzd";
            string url = "https://api.ipify.org/";
            //============
            // 解析 proxy 字串
            var parts = proxyStr.Split(':');
            if (parts.Length != 4)
            {
                Console.WriteLine("Proxy string format error");
                return;
            }
            string proxyHost = parts[0];
            int proxyPort = int.Parse(parts[1]);
            string proxyUser = parts[2];
            string proxyPass = parts[3];

            var proxy = new WebProxy(proxyHost, proxyPort)
            {
                Credentials = new NetworkCredential(proxyUser, proxyPass),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false
            };

            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = true,
            };

            using var httpClient = new HttpClient(httpClientHandler);

            try
            {
                
                var ip = await httpClient.GetStringAsync(url);
                Console.WriteLine($"IP from proxy: {ip}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task TestProxy_WebshareAsync2()
        {
            //string proxyStr = "142.147.128.93:6593:gngsucwj:zaw81idjdq11";
            string proxyStr = "dc.oxylabs.io:8005:abcd123_zmhMW:abcdT125+122";
            
            var parts = proxyStr.Split(':');
            string proxyHost = parts[0];
            int proxyPort = int.Parse(parts[1]);
            string proxyUser = parts[2];
            string proxyPass = parts[3];

            var proxy = new WebProxy(proxyHost, proxyPort)
            {
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false
            };

            var handler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = true,
                PreAuthenticate = true,
                UseDefaultCredentials = false
            };

            using var httpClient = new HttpClient(handler);

            // 加入 Proxy-Authorization Header (Basic Auth)
            string authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{proxyUser}:{proxyPass}"));
            httpClient.DefaultRequestHeaders.ProxyAuthorization = new AuthenticationHeaderValue("Basic", authValue);

            string url = "https://api.ipify.org/";

            try
            {
                string result = await httpClient.GetStringAsync(url);
                Console.WriteLine($"IP from proxy: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error2: {ex.Message}");
            }
        }
       
        public static async Task TestProxy_OxylabsAsync3()
        {
            string url = "https://api.ipify.org"; // or https://example.com/
            string proxyHost = "dc.oxylabs.io";
            int proxyPort = 8001;
            string username = "abcd123_zmhMW";
            string password = "abcdT125+122";

            await DownloadWeb_oxylab(url, proxyHost, proxyPort, username, password);
            
        }
        public static async Task TestProxy_WebshareAsync3()
        {
            string url = "https://api.ipify.org"; // or https://example.com/
            string proxyHost = "142.147.128.93";
            int proxyPort = 6593;
            string username = "lagengct";
            string password = "zaw81idjdq11";

            await DownloadWeb_webshare(url, proxyHost, proxyPort, username, password);
        }
        public static async Task TestProxy_WebshareAsync4()
        {
            string url = "https://api.ipify.org"; // or https://example.com/
            //"http://ipv4.webshare.io/"
            var client = new WebClient();
            client.Proxy = new WebProxy("23.95.150.145:6114");

            client.Proxy.Credentials =
            new NetworkCredential("lagengct", "x1m482lfqwzd");
            Console.WriteLine(client.DownloadString(url));

        }
        public static async Task TestProxy_WebshareAsync5()
        {
            string px_webshare = "142.147.128.93:6593:lagengct:x1m482lfqwzd";
            string px = "dc.oxylabs.io:8005:abcd123_zmhMW:abcdT125+122";
            
            string url = "https://api.ipify.org";

            // Parse proxy string
            var parts = px.Split(':');
            string proxyHost = parts[0];
            int proxyPort = int.Parse(parts[1]);
            string proxyUser = parts[2];
            string proxyPass = parts[3];

            string result = await GetViaProxyAsync(url, proxyHost, proxyPort, proxyUser, proxyPass);
            Console.WriteLine($"Response via proxy: {result}");

        }

        public static async Task<string> GetViaProxyAsync(string url, string proxyHost, int proxyPort, string proxyUser, string proxyPass)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Proxy = new WebProxy($"{proxyHost}:{proxyPort}")
            {
                Credentials = new NetworkCredential(proxyUser, proxyPass)
            };

            // Optional: set timeouts, user-agent etc.
            request.Timeout = 10000; // 10 seconds
            request.ReadWriteTimeout = 10000;
            request.UserAgent = "Mozilla/5.0 (compatible; ProxyHttpClient/1.0)";

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public static async Task DownloadWeb_webshare(string url, string proxyHost, int proxyPort, string username, string password)
        {  
            // Set up proxy
            var proxy = new WebProxy(proxyHost, proxyPort)
            {
                Credentials = new NetworkCredential(username, password)
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true
            };

            using (var client = new HttpClient(httpClientHandler))
            {
                var resp = await client.GetStringAsync(url);
                Console.WriteLine($"{proxyHost} {proxyPort} {resp}");
            }
        }
        public static async Task DownloadWeb_oxylab(string url, string proxyHost, int proxyPort, string username, string password)
        {

            // Adjust username format like Python version
            username = $"user-{username}-country-US";
            proxyHost = "dc.oxylabs.io"; // hardcoded like Python version

            // Set up proxy
            var proxy = new WebProxy(proxyHost, proxyPort)
            {
                Credentials = new NetworkCredential(username, password)
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true
            };

            using (var client = new HttpClient(httpClientHandler))
            {
                var resp = await client.GetStringAsync(url);
                Console.WriteLine($"{proxyHost} {proxyPort} {resp}");
            }
        }
    }
}
