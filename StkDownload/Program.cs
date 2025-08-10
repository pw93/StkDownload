using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using ProfitWin.Logging;
using StkDownload;

class Program
{

    

   

    //StkDownload fname_config
    static async Task Main(string[] args)
    {        
        if (true)
        {
            //for: 這行會註冊額外的編碼支援（包含 big5、gb2312 等）。
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Logger.Init();
            var runner = new DownloadRunner();
            string configFile = args.Length > 0 ? args[0] : "";
            await runner.RunAsync(configFile);
        }
    }
}
