using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfitWin.Logging;

namespace StkDownload
{
    public abstract class DatasetProviderTemplateA : IDatasetProvider
    {
        protected abstract string DatasetName { get; }
        protected abstract string RawDataDir { get; }
        protected abstract string BackupDataDir { get; }

        protected abstract string EncodingSrc { get; }
        protected abstract bool IsForceDownload { get; }
        protected abstract string GetDownloadUrl(string sid, string exchange);

        public bool IsForceDownloadEnabled() => IsForceDownload;

        public ConcurrentQueue<DownloadJob> GetDownloadItems(ForceDownloadMode download_mode)
        {
            bool is_force_download = download_mode switch
            {
                ForceDownloadMode.Yes => true,
                ForceDownloadMode.No => false,
                ForceDownloadMode.ByDataset => IsForceDownload,
                _ => throw new ArgumentException("Invalid download mode")
            };

            string FName_SE = Config.FName_SE;
            string FName_OTC = Config.FName_OTC;

            var sids_se = Utils.ReadTsv(FName_SE, remove_empty_row: true, trim_data_string: true);
            var sids_otc = Utils.ReadTsv(FName_OTC, remove_empty_row: true, trim_data_string: true);

            var sids = new List<(string id, string exchange)>();
            foreach (var row in sids_se)
            {
                if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]))
                {
                    Logger.loge("row.Count == 0 || string.IsNullOrWhiteSpace(row[0])");
                    continue;
                }
                sids.Add((row[0], "TW"));
            }
            foreach (var row in sids_otc)
            {
                if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]))
                {
                    Logger.loge("row.Count == 0 || string.IsNullOrWhiteSpace(row[0])");
                    continue;
                }
                sids.Add((row[0], "TWO"));
            }            

            var itemsQueue = new ConcurrentQueue<DownloadJob>();

            foreach (var d in sids)
            {
                string sid = d.id;
                string exchange = d.exchange;
                string url = GetDownloadUrl(sid, exchange);
                string filename = Path.Combine(RawDataDir, $"{sid}.html");

                if (File.Exists(filename) && !is_force_download)
                    continue;

                itemsQueue.Enqueue(new DownloadJob
                {
                    Url = url,
                    Filename = filename,
                    EncodingSrc = EncodingSrc,
                    
                    //Info = $"{this.GetType().Name}:{sid}",
                    Info = $"{DatasetName}:{sid}",                    
                });
            }

            Logger.logi($"Total download items: {itemsQueue.Count}");

            return itemsQueue;
        }

        public bool BackupDataset()
        {
            if (!Directory.Exists(RawDataDir))
            {
                Logger.loge($"Source directory does not exist: {RawDataDir}");
                return false;
            }

            try
            {
                if (Directory.Exists(BackupDataDir))
                {
                    Logger.logi($"Backup directory already exists, deleting: {BackupDataDir}");
                    Directory.Delete(BackupDataDir, true);
                }
                Utils.CopyDirectory(RawDataDir, BackupDataDir);
                Logger.logi($"Backup completed from {RawDataDir} to {BackupDataDir}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.loge($"Backup failed: {ex.Message}");
                return false;
            }
        }

        public bool ClearDataset()
        {
            if (Directory.Exists(RawDataDir))
            {
                Logger.logi($"Deleting: {RawDataDir}");
                bool r = Utils.DeleteDirectory(RawDataDir);
                if (!r)
                {
                    Logger.loge("Error ClearDataset: delete dataset");
                    return false;
                }
            }
            return true;
        }
    }
}
