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
                DatasetName.FubonIs => new DatasetProvider_FubonIs(),
                DatasetName.YahooProfile => new DatasetProvider_YahooProfile(),                
                _ => throw new ArgumentException($"Unsupported dataset name: {name}")
            };
        }
    }
}
