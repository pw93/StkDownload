using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using ProfitWin.Logging;
using System.Reflection.Metadata;
using System.Text;

namespace StkDownload
{
    public class DownloadAgentLocal : IDownloadAgent
    {
        private ConcurrentQueue<DownloadJob> _tasks;

        public DownloadAgentLocal()
        {
            
        }

        public async Task RunAsync(ConcurrentQueue<DownloadJob> jobs, ConcurrentQueue<DownloadJob> jobs_ok, ConcurrentQueue<DownloadJob> jobs_fail)
        {
            _tasks = jobs;
            using var httpClient = new HttpClient();

            while (_tasks.TryDequeue(out var task))
            {
                try
                {
                    task.TriedTimes++;
                    if(string.IsNullOrWhiteSpace(task.Info))
                        Logger.logi($"[Local] Downloading {task.Url} ");
                    else
                        Logger.logi($"[Local] Downloading {task.Info}");

                    //var content = await httpClient.GetStringAsync(task.Url);

                    var bytes = await httpClient.GetByteArrayAsync(task.Url);

                    Encoding enc;
                    if (string.IsNullOrEmpty(task.EncodingSrc))
                        enc = Encoding.UTF8;
                    else
                        enc = Encoding.GetEncoding(task.EncodingSrc);
                    string content = enc.GetString(bytes);                    

                    var directory = Path.GetDirectoryName(task.Filename);
                    Directory.CreateDirectory(directory);

                    await File.WriteAllTextAsync(task.Filename, content, System.Text.Encoding.UTF8);                    

                    Logger.logi($"[Local] Saved {task.Filename}");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrWhiteSpace(task.Info))
                        Logger.logw($"[Local] Error downloading {task.Url} (attempt {task.TriedTimes}/{task.RetryCount}): {ex.Message}");
                    else
                        Logger.logw($"[Local] Error downloading {task.Info} (attempt {task.TriedTimes}/{task.RetryCount}): {ex.Message}");

                    if (task.TriedTimes < task.RetryCount)
                    {
                        _tasks.Enqueue(task); // 失敗且還沒超過重試上限，放回佇列等待下次
                    }
                    else
                    {
                        Logger.loge($"[Local] Give up downloading {task.Url} after {task.TriedTimes} attempts.");
                    }
                }
                
                await Task.Delay(TimeSpan.FromSeconds(task.CoolingTime));
            }
        }
    }
}
