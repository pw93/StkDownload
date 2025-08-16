using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProfitWin.Logging;

namespace StkDownload
{                                           
    public class DatasetProvider_FubonIs : DatasetProvider_TemplateA
    {
        protected override string EncodingSrc => "big5";
        static string aa = Config.FName_Proxy1;
        protected override string DatasetName => "FubonIs";
        protected override string RawDataDir => Config.DName_Raw_fubon_is;
        protected override string BackupDataDir => Config.DName_Raw_fubon_is_backup;        
        protected override bool IsForceDownload => Config.IsForceDownload_fubon_is;

        protected override string GetDownloadUrl(string sid, string exchange)
        {
            return $"https://fubon-ebrokerdj.fbs.com.tw/z/zc/zcq/zcq_{sid}.djhtm";
        }
    }
}
