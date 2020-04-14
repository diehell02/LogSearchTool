using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace LogSearchTool.Utils
{
    class FileUtil
    {
        public static void Extra(DirectoryInfo directoryInfo)
        {
            directoryInfo?.EnumerateFiles().ToList().ForEach(fileInfo =>
            {
                Extra(fileInfo);
            });

            directoryInfo?.EnumerateDirectories().ToList().ForEach(directoryInfo =>
            {
                Extra(directoryInfo);
            });
        }

        private static void Extra(FileInfo fileInfo)
        {
            if (fileInfo.FullName.EndsWith(".log.zip"))
            {
                UnZipFile.GzipDecompress(fileInfo);
            }
            else if (fileInfo.Extension == ".zip")
            {
                UnZipFile.UnZip(fileInfo);
            }
            
        }
    }
}
