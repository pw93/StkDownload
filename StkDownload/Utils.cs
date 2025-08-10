using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StkDownload
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection.Metadata;

    public static class Utils
    {
        public static List<List<string>> ReadTsv(string filepath, bool remove_empty_row = false, bool trim_data_string=false)
        {
            var lines = new List<List<string>>();

            foreach (var line in File.ReadLines(filepath))
            {
                //if (string.IsNullOrWhiteSpace(line))
                //    continue;

                var columns = line.Split('\t').ToList();

                if (trim_data_string)
                    columns = columns.Select(c => c.Trim()).ToList();

                if (remove_empty_row && columns.All(string.IsNullOrWhiteSpace))
                    continue;

                lines.Add(columns);


            }

            return lines;
        }

        /// <summary>
        /// 讀取分隔符分隔的純文字檔，忽略空行與以 # 開頭的註解行。
        /// </summary>
        /// <param name="filepath">檔案路徑</param>
        /// <param name="separator">欄位分隔符號，例如 '\t' 或 ':'</param>
        /// <returns>回傳檔案中每行拆分後的欄位清單</returns>
        public static List<List<string>> ReadSeparatedFile(string filepath, char separator)
        {
            var lines = new List<List<string>>();

            foreach (var line in File.ReadLines(filepath))
            {
                // 跳過空白行
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // 跳過以 # 開頭的註解行
                if (line.StartsWith("#"))
                    continue;

                var columns = line.Split(separator).ToList();
                lines.Add(columns);
            }

            return lines;
        }

        public static bool DeleteDirectory(string dirName)
        {
            try
            {
                if (Directory.Exists(dirName))
                {
                    Directory.Delete(dirName, recursive: true);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete directory '{dirName}': {ex.Message}");
                return false;
            }
        }

        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            if (!Directory.Exists(sourceDirName))
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirName}");

            Directory.CreateDirectory(destDirName);

            foreach (var file in Directory.GetFiles(sourceDirName))
            {
                string destFile = Path.Combine(destDirName, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var directory in Directory.GetDirectories(sourceDirName))
            {
                string destSubDir = Path.Combine(destDirName, Path.GetFileName(directory));
                CopyDirectory(directory, destSubDir);
            }
        }


        

    }

}
