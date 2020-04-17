using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogSearchTool.Utils
{
    class SearchUtil
    {
        public static void SearchKeyWord(string keyWord, DirectoryInfo directoryInfo, List<string> filesToInclude, 
            List<string> filesToExclude, Action<string> callback)
        {
            Task.Factory.StartNew(() =>
            {
                Search(keyWord, directoryInfo, filesToInclude, filesToExclude, callback);
            });
        }

        private static void Search(string keyWord, DirectoryInfo directoryInfo, List<string> filesToInclude, 
            List<string> filesToExclude, Action<string> callback)
        {
            directoryInfo?.EnumerateFiles().ToList().ForEach(fileInfo =>
            {
                bool ignore = false;

                foreach (var fileExtension in filesToInclude) {
                    if (fileInfo.Extension != fileExtension) {
                        ignore = true;
                        break;
                    }
                }

                foreach (var fileExtension in filesToExclude) {
                    if (fileInfo.Extension == fileExtension) {
                        ignore = true;
                        break;
                    }
                }

                if (ignore) {
                    return;
                }

                using (StreamReader sr = fileInfo?.OpenText())
                {
                    string s = "";
                    while (!string.IsNullOrEmpty(s = sr.ReadLine()))
                    {
                        if(s.Contains(keyWord, StringComparison.InvariantCultureIgnoreCase))
                        {
                            callback?.Invoke(s);
                        }
                    }
                }
            });

            directoryInfo?.EnumerateDirectories().ToList().ForEach(directoryInfo =>
            {
                Search(keyWord, directoryInfo, filesToInclude, filesToExclude, callback);
            });
        }
    }
}
