using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public enum DatasetName
    {
        YahooMajorHolders = 1,
        YahooProfile,
        FubonIs,
    }
    public enum ForceDownloadMode
    {
        Yes,
        No,
        ByDataset
    }
    public interface IDatasetProvider
    {
        ConcurrentQueue<DownloadJob> GetDownloadItems(ForceDownloadMode download_mode);
        bool BackupDataset();
        bool ClearDataset();
        bool IsForceDownloadEnabled(); //true: force, false: fill mode
    }
}
