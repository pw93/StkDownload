using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public interface IDownloadAgent
    {
        Task RunAsync(ConcurrentQueue<DownloadJob> jobs, ConcurrentQueue<DownloadJob> jobs_ok, ConcurrentQueue<DownloadJob> jobs_fail);
        
    }
}
