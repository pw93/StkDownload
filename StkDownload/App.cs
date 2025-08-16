using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static StkDownload.DownloadRunner;

namespace StkDownload
{
    public class App
    {
        public static List<ProxyConfig> LoadProxies(string filename)
        {
            var proxies = new List<ProxyConfig>();
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Proxy config file not found: {filename}");
                return proxies;
            }
            try
            {
                var proxyLines = Utils.ReadSeparatedFile(filename, ':');
                foreach (var line in proxyLines)
                {
                    if (line.Count < 4) continue;
                    if (!int.TryParse(line[1], out int port)) continue;

                    proxies.Add(new ProxyConfig
                    {
                        Host = line[0],
                        Port = port,
                        User = line[2],
                        Password = line[3]
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load proxies from {filename}: {ex.Message}");
            }
            return proxies;
        }
        public static async Task check_proxy()
        {
            var downloaders = new List<IDownloadAgent>();
            var proxies1 = LoadProxies(Config.FName_Proxy1);
            var proxies2 = LoadProxies(Config.FName_Proxy2);
            var allProxies = proxies1.Concat(proxies2).ToList();
            
            {
                
            }
            var tasks = new List<Task>();
            var jobsTest = new ConcurrentQueue<DownloadJob>();
            var jobsQueueOk = new ConcurrentQueue<DownloadJob>();
            var jobsQueueFail = new ConcurrentQueue<DownloadJob>();

            int i;
            //for (i = 0; i < downloaders.Count; i++)
            foreach (var proxy in allProxies)
            {
                downloaders.Add(new DownloadAgentProxy(proxy.Host, proxy.Port, proxy.User, proxy.Password));
                jobsTest.Enqueue(new DownloadJob
                {
                    Url = "https://api.ipify.org",
                    //@@
                    Filename = $"c:\\temp\\proxy_test_{proxy.Host}_{proxy.Port}.html",
                    EncodingSrc = "utf-8",
                    Info = $"{proxy.Host}:{proxy.Port}",
                    RetryCount = 1
                });
            }

            foreach (var downloader in downloaders)
            {
                tasks.Add(downloader.RunAsync(jobsTest, jobsQueueOk, jobsQueueFail));
            }
            await Task.WhenAll(tasks);

        }
    }
}
