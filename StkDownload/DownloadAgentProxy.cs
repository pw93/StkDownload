using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfitWin.Logging;

namespace StkDownload
{
    public class DownloadAgentProxy : IDownloadAgent
    {
        private ConcurrentQueue<DownloadJob> _tasks;
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public DownloadAgentProxy(string host, int port, string username, string password)
        {
            
            _host = host;
            _port = port;
            _username = username;
            _password = password;
        }

        public async Task RunAsync(ConcurrentQueue<DownloadJob> jobs, ConcurrentQueue<DownloadJob> jobs_ok, ConcurrentQueue<DownloadJob> jobs_fail)
        {
            _tasks = jobs;
            using var httpClient = CreateHttpClientWithProxy();

            int consecutiveFailures = 0; // 連續失敗計數器

            while (_tasks.TryDequeue(out var task))
            {
                try
                {
                    task.TriedTimes++; // 嘗試次數 +1
                    if (string.IsNullOrWhiteSpace(task.Info))
                        Logger.logi($"[Proxy {_host}] Downloading {task.Url} ");
                    else
                        Logger.logi($"[Proxy {_host}] Downloading {task.Info}");
                    
                    var bytes = await httpClient.GetByteArrayAsync(task.Url);

                    Encoding enc;
                    if (string.IsNullOrEmpty(task.EncodingSrc))
                        enc = Encoding.UTF8;
                    else
                        enc = Encoding.GetEncoding(task.EncodingSrc);
                    string content = enc.GetString(bytes);

                    var directory = Path.GetDirectoryName(task.Filename);                    
                    Directory.CreateDirectory(directory);

                    await File.WriteAllTextAsync(task.Filename, content, Encoding.UTF8);

                    Logger.logi($"[Proxy {_host}] Saved {task.Filename}");


                    consecutiveFailures = 0; // 成功後重設連續失敗計數
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrWhiteSpace(task.Info))
                        Logger.logw($"[Proxy {_host}] Error downloading {task.Url} (attempt {task.TriedTimes}/{task.RetryCount}): {ex.Message}");
                    else
                        Logger.logw($"[Proxy {_host}] Error downloading {task.Info} (attempt {task.TriedTimes}/{task.RetryCount}): {ex.Message}");

                    consecutiveFailures++; // 連續失敗次數 +1

                    if (consecutiveFailures >= 10)
                    {
                        Logger.loge($"[Proxy {_host}] Consecutive failure limit reached (10 times). Stopping downloads.");
                        break; // 超過失敗限制，跳出 while 迴圈結束任務
                    }

                    if (task.TriedTimes < task.RetryCount)
                    {
                        Logger.logw($"[Proxy {_host}] Re-enqueue {task.Url} for next try (tried {task.TriedTimes} times)");
                        _tasks.Enqueue(task);
                    }
                    else
                    {
                        Logger.loge($"[Proxy {_host}] Give up downloading {task.Url} after {task.TriedTimes} attempts.");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(task.CoolingTime));
            }
        }


        private HttpClient CreateHttpClientWithProxy()
        {
            var handler = new HttpClientHandler()
            {
                Proxy = new System.Net.WebProxy($"{_host}:{_port}")
                {
                    Credentials = new System.Net.NetworkCredential(_username, _password)
                },
                UseProxy = true
            };

            return new HttpClient(handler);
        }
    }

}