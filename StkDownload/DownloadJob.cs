using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public enum DownloadStatus
    {        
        Ok,
        Fail,
        None,
    }
    public class DownloadJob
    {
        public string Url { get; set; } = "";
        public string Filename { get; set; } = "";
        public double CoolingTime { get; set; } = 5; //5s        
        public int RetryCount { get; set; } = 3;
        public string Info { get; set; } = "";        

        // 紀錄來源檔案編碼 (ex: "utf-8", "big5", "gb2312"等)
        public string EncodingSrc { get; set; } = "";

        //================================
        //DO NOT set, "TriedTimes" is for downloader to count when running
        public int TriedTimes { get; set; } = 0;
        //DO NOT set,
        public DownloadStatus Status { get; set; } = DownloadStatus.None;


        public DownloadJob(string url = "", string filename = "", double coolingTimeSeconds = 5)
        {
            Url = url;
            Filename = filename;
            CoolingTime = coolingTimeSeconds;
        }
    }

}
