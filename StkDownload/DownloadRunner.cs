using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public class DownloadRunner
    {
        public class ProxyConfig
        {
            public string Host { get; set; } = "";
            public int Port { get; set; } = 0;
            public string User { get; set; }  = "";
            public string Password { get; set; } = "";
        }
        public DatasetName DatasetName { get; set; }
        public bool IsUseProxy { get; set; } = true;
        public ForceDownloadMode forceDownloadMode { get; set; } = ForceDownloadMode.ByDataset;

        // 你可以加入其他設定，比如 proxy config path, etc.

        public bool SetData(string fname_config)
        {
            try
            {
                var data = Utils.ReadSeparatedFile(fname_config, ':');

                foreach (var d in data)
                {
                    if (d.Count < 2) continue;

                    var key = d[0].Trim().ToLower();
                    var val = d[1].Trim();

                    if (key == "dataset")
                    {
                        //Enum.TryParse<DatasetName>(val, true, out var ds) 這裡第二個參數 true 是 ignoreCase，表示 enum 轉換時是不區分大小寫的。
                        if (Enum.TryParse<DatasetName>(val, true, out var ds))
                            DatasetName = ds;
                        else
                            Console.WriteLine($"Warning: Unknown dataset name '{val}' in config.");
                    }
                    else if (key == "is_use_proxy")
                    {
                        if (bool.TryParse(val, out var b))
                            IsUseProxy = b;
                        else
                        {
                            Console.WriteLine($"Warning: Invalid boolean value for 'is_use_proxy': '{val}'. Expected 'true' or 'false'. Defaulting to false.");
                            IsUseProxy = false;
                        }
                    }
                    else if (key == "force_download_mode_global")
                    {
                        switch (val.ToLower())
                        {
                            case "yes":
                                forceDownloadMode = ForceDownloadMode.Yes;
                                break;
                            case "no":
                                forceDownloadMode = ForceDownloadMode.No;
                                break;
                            case "by_dataset":
                                forceDownloadMode = ForceDownloadMode.ByDataset;
                                break;
                            default:
                                Console.WriteLine($"ERROR: Unknown force_download_mode_global value '{val}', default to ByDataset.");
                                forceDownloadMode = ForceDownloadMode.ByDataset;
                                break;
                        }
                    }
                    else 
                    {
                        Console.WriteLine($"ERROR: key[{key}] is wrong.");
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read config file '{fname_config}': {ex.Message}");
                return false;
            }
        }

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

        /// <summary>
        /// 根據設定檔或預設設定，建立下載任務並啟動下載器執行所有下載
        /// </summary>
        /// <param name="fname_config">設定檔路徑，若空字串則使用預設 Config</param>
        public async Task RunAsync(string fname_config="")
        {            
            if (!string.IsNullOrEmpty(fname_config))
            {
                if (!SetData(fname_config))
                    return;
            }
            else
            {
                DatasetName = Config.download_dataset_name;
                IsUseProxy = Config.IsUseProxy;
                forceDownloadMode  = Config.ForceDownload_GlobalMode;

            }
            // 根據 DatasetName 產生對應的 DatasetProvider
            IDatasetProvider provider;
            try
            {
                provider = DatasetProviderFactory.Create(DatasetName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create dataset provider for '{DatasetName}': {ex.Message}");
                return;
            }

            if (forceDownloadMode == ForceDownloadMode.Yes ||
                (forceDownloadMode == ForceDownloadMode.ByDataset && provider.IsForceDownloadEnabled()) )
            {
                if (!provider.ClearDataset())
                {
                    Console.WriteLine($"ERROR: ClearDataset wrong");
                }
                
            }
                
            

            var itemsQueue = provider.GetDownloadItems(forceDownloadMode);
            var jobsQueueOk = new ConcurrentQueue<DownloadJob>();
            var jobsQueueFail = new ConcurrentQueue<DownloadJob>();

            var downloaders = new List<IDownloadAgent>();

            // 加本地下載器            
            var localAgent = new DownloadAgentLocal();
            downloaders.Add(localAgent);

            if (IsUseProxy)
            {
                var proxies1 = LoadProxies(Config.FName_Proxy1);
                var proxies2 = LoadProxies(Config.FName_Proxy2);
                var allProxies = proxies1.Concat(proxies2).ToList();

                foreach (var proxy in allProxies)
                {
                    downloaders.Add(new DownloadAgentProxy(proxy.Host, proxy.Port, proxy.User, proxy.Password));
                }
            }

            // 啟動所有下載器並等待完成
            var tasks = new List<Task>();
            foreach (var downloader in downloaders)
            {
                tasks.Add(downloader.RunAsync(itemsQueue, jobsQueueOk, jobsQueueFail));
            }
            await Task.WhenAll(tasks);

            //-----------------------------
            // Retry failed jobs by Local
            while (jobsQueueFail.TryDequeue(out var failedTask))
                itemsQueue.Enqueue(failedTask);

            await localAgent.RunAsync(itemsQueue, jobsQueueOk, jobsQueueFail);


            if (!provider.BackupDataset())
            {
                Console.WriteLine($"ERROR: BackupDataset wrong");
            }          

            Console.WriteLine("All downloads finished.");
        }
    }
}
