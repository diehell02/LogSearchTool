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
        public static void SearchKeyWord(string keyWord, DirectoryInfo directoryInfo, Action<string> callback)
        {
            Task.Factory.StartNew(() =>
            {
                Search(keyWord, directoryInfo, callback);
            });
        }

        private static void Search(string keyWord, DirectoryInfo directoryInfo, Action<string> callback)
        {
            directoryInfo?.EnumerateFiles().ToList().ForEach(fileInfo =>
            {
                using (StreamReader sr = fileInfo?.OpenText())
                {
                    string s = "";
                    while (!string.IsNullOrEmpty(s = sr.ReadLine()))
                    {
                        if(s.Contains(keyWord))
                        {
                            callback?.Invoke(s);
                        }
                    }
                }
            });

            directoryInfo?.EnumerateDirectories().ToList().ForEach(directoryInfo =>
            {
                Search(keyWord, directoryInfo, callback);
            });
        }
    }
}
