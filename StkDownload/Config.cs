using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public static class Config
    {
        public static string FName_Proxy1 { get; } = @"C:\data\code\aistk_system\webshare_1.txt";
        public static string FName_Proxy2 { get; } = @"C:\data\code\aistk_system\oxylabs_0.txt";
        public static string FName_SE { get; } = @"C:\data\code\aistk_system\database\aistk\ripe\info\se_normal.txt";
        public static string FName_OTC { get; } = @"C:\data\code\aistk_system\database\aistk\ripe\info\otc_normal.txt";
        //----------------------
        public static string DName_Raw_yahoo_major_holders { get; } = @"C:\data\code\aistk_system\database\aistk\raw\disp_yahoo";
        public static string DName_Raw_yahoo_major_holders_backup { get; } = @"C:\data\code\aistk_system\database\aistk\raw_backup\disp_yahoo";
        public static bool IsForceDownload_yahoo_major_holders { get; } = true;


        //----------------------
        public static string DName_Raw_fubon_is { get; } = @"C:\data\code\aistk_system\database\aistk\raw\fubon_is";
        public static string DName_Raw_fubon_is_backup { get; } = @"C:\data\code\aistk_system\database\aistk\raw_backup\fubon_is";
        public static bool IsForceDownload_fubon_is { get; } = true;


        //=====================================================
        //public static DatasetName download_dataset_name = DatasetName.YahooMajorHolders;
        public static DatasetName download_dataset_name = DatasetName.FubonIs;
        public static bool IsUseProxy { get; } = true;

        public static ForceDownloadMode ForceDownload_GlobalMode { get; } = ForceDownloadMode.ByDataset;


        //=====================================================
        //debug modificaton
        //public static ForceDownloadMode ForceDownload_GlobalMode { get; } = ForceDownloadMode.No;
        //public static bool IsUseProxy { get; } = false;




    }
}
