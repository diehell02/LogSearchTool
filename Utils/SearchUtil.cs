using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogSearchTool.Utils
{
    public class SearchUtil
    {
        public class SearchResult
        {
            private List<StringBuilder> _content = new List<StringBuilder>();

            public FileInfo FileInfo
            {
                get;
                set;
            }

            public List<StringBuilder> Content
            {
                get => _content;
            }
        }

        private static Regex rex = new Regex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}", RegexOptions.Compiled);

        public static async Task SearchKeyWord(string keyWord, DirectoryInfo directoryInfo,
            List<string> filesToInclude, List<string> filesToExclude, Action<IList<SearchResult>> resultCallback)
        {
            await Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();

                Search(keyWord, directoryInfo, filesToInclude, filesToExclude, resultCallback);

                stopwatch.Stop();

                Console.WriteLine($"Search Finish, ElapsedTime:{stopwatch.ElapsedMilliseconds}");
            });
        }

        private static bool IsLogLineStart(string line, Regex rex)
        {
            var match = rex.Match(line.Substring(0, 18));

            if (match.Success)
            {
                return false;
            }

            return true;
        }

        private static void Search(string keyWord, DirectoryInfo directoryInfo, List<string> filesToInclude,
            List<string> filesToExclude, Action<IList<SearchResult>> resultCallback)
        {
            Console.WriteLine($"{DateTime.Now} Enter Search");
            var filesList = directoryInfo?.EnumerateFiles().Where(fileInfo =>
            {
                foreach (var fileExtension in filesToInclude)
                {
                    if (fileInfo.Extension != fileExtension)
                    {
                        return false;
                    }
                }

                foreach (var fileExtension in filesToExclude)
                {
                    if (fileInfo.Extension == fileExtension)
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();
            Console.WriteLine($"{DateTime.Now} Finish Filter");
            var resultLock = new object();
            var maxParallelFilesNumber = 50;

            filesList.Sort((file1, file2) =>
            {
                return file1.Name.CompareTo(file2.Name);
            });

            Console.WriteLine($"{DateTime.Now} Finish Sort");

            var index = 0;
            while (index < filesList.Count)
            {
                var dealCount = filesList.Count - index;
                var count = dealCount < maxParallelFilesNumber ? dealCount : maxParallelFilesNumber;
                var dealFilesList = filesList.GetRange(index, count);
                var searchResults = new List<SearchResult>();

                Parallel.ForEach(dealFilesList, new ParallelOptions() { MaxDegreeOfParallelism = maxParallelFilesNumber }, fileInfo =>
                {
                    using (StreamReader sr = fileInfo?.OpenText())
                    {
                        string s = "";
                        var lastLineFoundFlag = false;
                        var currentSearchResult = new SearchResult()
                        {
                            FileInfo = fileInfo,
                        };

                        while (!string.IsNullOrEmpty(s = sr.ReadLine()))
                        {
                            if (lastLineFoundFlag)
                            {
                                if (!IsLogLineStart(s, rex))
                                {
                                    currentSearchResult.Content.Last().AppendLine(s);
                                    continue;
                                }
                            }

                            if (s.Contains(keyWord))
                            {
                                lastLineFoundFlag = true;
                                currentSearchResult.Content.Add(new StringBuilder(s));
                            }
                            else
                            {
                                lastLineFoundFlag = false;
                            }
                        }

                        if (currentSearchResult.Content.Count > 0)
                        {
                            searchResults.Add(currentSearchResult);
                        }
                    }
                });

                Console.WriteLine($"{DateTime.Now} Finish Parallel Search");

                searchResults.Sort((file1, file2) =>
                {
                    return file1.FileInfo.Name.CompareTo(file2.FileInfo.Name);
                });

                Console.WriteLine($"{DateTime.Now} Finish SearchResult Sort");

                resultCallback.Invoke(searchResults);

                index += count;
            }

            var directories = directoryInfo?.EnumerateDirectories().ToList();

            foreach (var directory in directories)
            {
                Search(keyWord, directory, filesToInclude, filesToExclude, resultCallback);
            }
        }
    }
}