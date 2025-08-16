using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public static class Config
    {
        public static string DName_DBBase { get; } = @"C:\data\code\aistk_system\database\aistk";
        public static string DName_Raw { get; } = Path.Combine(DName_DBBase, "raw");
        public static string DName_RawBackup { get; } = Path.Combine(DName_DBBase, "raw_backup");
        public static string DName_Ripe { get; } = Path.Combine(DName_DBBase, "ripe");

        public static string FName_Proxy1 { get; } = @"C:\data\code\aistk_system\webshare_2.txt";
        public static string FName_Proxy2 { get; } = @"C:\data\code\aistk_system\oxylabs_0.txt";
        public static string FName_SE { get; } = Path.Combine(DName_Ripe, @"info\se_normal.txt");
        public static string FName_OTC { get; } = Path.Combine(DName_Ripe, @"info\otc_normal.txt");
        //----------------------
        public static string DName_Raw_yahoo_major_holders { get; } = Path.Combine(DName_Raw, "disp_yahoo");
        public static string DName_Raw_yahoo_major_holders_backup { get; } = Path.Combine(DName_RawBackup, "disp_yahoo");
        public static bool IsForceDownload_yahoo_major_holders { get; } = true;

        public static string DName_Raw_yahoo_profile { get; } = Path.Combine(DName_Raw, "yahoo_profile");
        public static string DName_Raw_yahoo_profile_backup { get; } = Path.Combine(DName_RawBackup, "yahoo_profile");
        public static bool IsForceDownload_yahoo_profile { get; } = true;


        //----------------------
        public static string DName_Raw_fubon_is { get; } = Path.Combine(DName_Raw, "fubon_is");
        public static string DName_Raw_fubon_is_backup { get; } = Path.Combine(DName_RawBackup, "fubon_is");
        public static bool IsForceDownload_fubon_is { get; } = true;


        //=====================================================
        //public static DatasetName download_dataset_name = DatasetName.YahooMajorHolders;
        //public static DatasetName download_dataset_name = DatasetName.FubonIs;
        //public static DatasetName download_dataset_name = DatasetName.YahooProfile;
        public static bool IsUseProxy { get; } = true;

        public static ForceDownloadMode ForceDownload_GlobalMode { get; } = ForceDownloadMode.ByDataset;


        //=====================================================
        //debug modificaton
        //public static ForceDownloadMode ForceDownload_GlobalMode { get; } = ForceDownloadMode.No;
        //public static bool IsUseProxy { get; } = false;
        public static DatasetName download_dataset_name = DatasetName.FubonIs;




    }
}
