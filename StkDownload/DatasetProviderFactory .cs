using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    public static class DatasetProviderFactory
    {
        
        public static IDatasetProvider Create(DatasetName name)
        {
            return name switch
            {
                DatasetName.YahooMajorHolders => new DatasetProvider_YahooMajorHolders(),
                DatasetName.FubonIs => new DatasetProvider_FubonIs(),  // 你要另外實作
                //DatasetName.YahooProfile => new DatasetProvider_FubonIs(),            // 你要另外實作
                _ => throw new ArgumentException($"Unsupported dataset name: {name}")
            };
        }
    }
}
