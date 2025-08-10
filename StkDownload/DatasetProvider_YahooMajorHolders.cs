using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfitWin.Logging;

namespace StkDownload
{
    public class DatasetProvider_YahooMajorHolders : IDatasetProvider
    {
        public bool IsForceDownloadEnabled()
        {
            return Config.IsForceDownload_yahoo_major_holders;
        }
        public ConcurrentQueue<DownloadJob> GetDownloadItems(ForceDownloadMode download_mode)
        {
            //=======================================
            //config
            //var fname_se = @"C:\data\code\aistk_system\database\aistk\ripe\info\se_normal.txt";
            //var fname_otc = @"C:\data\code\aistk_system\database\aistk\ripe\info\otc_normal.txt";
            //var dname_raw = @"C:\data\code\aistk_system\database\aistk\raw\disp_yahoo_test";
            var fname_se = Config.FName_SE;
            var fname_otc = Config.FName_OTC;
            var dname_raw = Config.DName_Raw_yahoo_major_holders;            
            //=======================================
            bool is_force_download = download_mode switch
            {
                ForceDownloadMode.Yes => true,
                ForceDownloadMode.No => false,
                ForceDownloadMode.ByDataset => Config.IsForceDownload_yahoo_major_holders,
                _ => throw new ArgumentException("Invalid download mode")
            };



            var sids_se = Utils.ReadTsv(fname_se);
            var sids_otc = Utils.ReadTsv(fname_otc);

            var sids = new List<(string id, string exchange)>();
            foreach (var sid in sids_se)
            {
                sids.Add((sid[0], "TW"));
            }
            foreach (var sid in sids_otc)
            {
                sids.Add((sid[0], "TWO"));
            }

            var itemsQueue = new ConcurrentQueue<DownloadJob>();

            foreach (var row in sids)
            {
                string sid = row.id;
                string exchange = row.exchange;
                string url = $"https://tw.stock.yahoo.com/quote/{sid}.{exchange}/major-holders";
                string filename = Path.Combine(dname_raw, $"{sid}.html");

                if (File.Exists(filename) && !is_force_download)
                    continue;

                itemsQueue.Enqueue(new DownloadJob
                {
                    Url = url,
                    Filename = filename,
                    Info = $"major-holders:{sid}",
                });
            }

            Logger.logi($"Total download items: {itemsQueue.Count}");

            return itemsQueue;
        }
        public bool BackupDataset()
        {
            string sourceDir = Config.DName_Raw_yahoo_major_holders;
            string backupDir = Config.DName_Raw_yahoo_major_holders_backup;

            if (!Directory.Exists(sourceDir))
            {
                Logger.loge($"Source directory does not exist: {sourceDir}");
                return false;
            }

            try
            {
                if (Directory.Exists(backupDir))
                {
                    Logger.logi($"Backup directory already exists, deleting: {backupDir}");
                    Directory.Delete(backupDir, true);
                }
                Utils.CopyDirectory(sourceDir, backupDir);
                Logger.logi($"Backup completed from {sourceDir} to {backupDir}");
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
            string sourceDir = Config.DName_Raw_yahoo_major_holders;
            if (Directory.Exists(sourceDir))
            {
                Logger.logi($"deleting: {sourceDir}");
                bool r = Utils.DeleteDirectory(sourceDir);
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