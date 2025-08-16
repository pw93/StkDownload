using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//add dataset code:
//DatasetProviderFactory
namespace StkDownload
{   
    public class DatasetProvider_YahooProfile : DatasetProvider_TemplateA
    {
        protected override string EncodingSrc => "utf-8";
        protected override string DatasetName => "YahooProfile";
        protected override string RawDataDir => Config.DName_Raw_yahoo_profile;
        protected override string BackupDataDir => Config.DName_Raw_yahoo_profile_backup;
        protected override bool IsForceDownload => Config.IsForceDownload_yahoo_profile;

        protected override string GetDownloadUrl(string sid, string exchange)
        {   
            return $"https://tw.stock.yahoo.com/quote/{sid}.{exchange}/profile";
        }
    }
}
