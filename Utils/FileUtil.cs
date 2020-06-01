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
        public static void Decompress(DirectoryInfo directoryInfo)
        {
            directoryInfo?.EnumerateFiles().ToList().ForEach(fileInfo =>
               {
                   Decompress(fileInfo);
               });

            directoryInfo?.EnumerateDirectories().ToList().ForEach(directoryInfo =>
            {
                Decompress(directoryInfo);
            });
        }

        private static void Decompress(FileInfo fileInfo)
        {
            var fileTypes = MimeTypeUtil.GetMimeType(fileInfo.FullName);

            if (fileTypes.Contains("gz"))
            {
                UnZipFile.GzipDecompress(fileInfo);
                DeleteFile(fileInfo);
            }
            else if (fileTypes.Contains("zip"))
            {
                UnZipFile.UnZip(fileInfo);
                DeleteFile(fileInfo);
            }
        }

        public static DirectoryInfo CreateDecompressDirectory(DirectoryInfo directoryInfo)
        {
            var newDirectoryPath = $"{directoryInfo.Parent.FullName}/Decompress";
            DirectoryInfo decompressDirectory = Directory.CreateDirectory(newDirectoryPath);

            CopyAllFiles(directoryInfo, decompressDirectory);

            return decompressDirectory;
        }

        private static void CopyAllFiles(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory)
        {
            sourceDirectory?.EnumerateFiles().ToList().ForEach(fileInfo =>
            {
                var newFilePath = $"{destinationDirectory.FullName}/{fileInfo.Name}";

                File.Copy(fileInfo.FullName, newFilePath, true);
            });

            sourceDirectory?.EnumerateDirectories().ToList().ForEach(directoryInfo =>
            {
                var subDestinationDirectoryPath = $"{destinationDirectory.FullName}/{directoryInfo.Name}";
                DirectoryInfo subDestinationDirectory = Directory.CreateDirectory(subDestinationDirectoryPath);

                CopyAllFiles(directoryInfo, subDestinationDirectory);
            });
        }

        private static void DeleteFile(FileInfo fileInfo)
        {
            File.Delete(fileInfo.FullName);
        }
    }
}
