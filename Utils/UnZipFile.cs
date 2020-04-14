using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace LogSearchTool.Utils
{
	class UnZipFile
	{
        //public static void UnZip(string filePath, string directoryName = "")
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        Console.WriteLine("Cannot find file '{0}'", filePath);
        //        return;
        //    }

        //    ZipFile zipFile = new ZipFile(File.OpenRead(filePath));

        //    using (ZipInputStream s = new ZipInputStream(File.OpenRead(filePath)))
        //    {
        //        ZipEntry theEntry;
        //        while ((theEntry = s.GetNextEntry()) != null)
        //        {

        //            Console.WriteLine(theEntry.Name);

        //            if (string.IsNullOrEmpty(directoryName))
        //            {
        //                directoryName = Path.GetDirectoryName(theEntry.Name);
        //            }

        //            string fileName = Path.GetFileName(theEntry.Name);

        //            // create directory
        //            if (directoryName.Length > 0)
        //            {
        //                Directory.CreateDirectory(directoryName);
        //            }

        //            if (fileName != String.Empty)
        //            {
        //                using (FileStream streamWriter = File.Create($"{directoryName}/{theEntry.Name}"))
        //                {

        //                    int size = 2048;
        //                    byte[] data = new byte[2048];
        //                    while (true)
        //                    {
        //                        size = s.Read(data, 0, data.Length);
        //                        if (size > 0)
        //                        {
        //                            streamWriter.Write(data, 0, size);
        //                        }
        //                        else
        //                        {
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //#region 解压  

        ///// <summary>   
        ///// 解压功能(解压压缩文件到指定目录)   
        ///// </summary>   
        ///// <param name="fileToUnZip">待解压的文件</param>   
        ///// <param name="zipedFolder">指定解压目标目录</param>   
        ///// <param name="password">密码</param>   
        ///// <returns>解压结果</returns>   
        //public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        //{
        //    bool result = true;
        //    FileStream fs = null;
        //    ZipInputStream zipStream = null;
        //    ZipEntry ent = null;
        //    string fileName;

        //    if (!File.Exists(fileToUnZip))
        //        return false;

        //    if (!Directory.Exists(zipedFolder))
        //        Directory.CreateDirectory(zipedFolder);

        //    try
        //    {
        //        zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
        //        if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
        //        while ((ent = zipStream.GetNextEntry()) != null)
        //        {
        //            if (!string.IsNullOrEmpty(ent.Name))
        //            {
        //                fileName = Path.Combine(zipedFolder, ent.Name);
        //                fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi   

        //                if (fileName.EndsWith("\\"))
        //                {
        //                    Directory.CreateDirectory(fileName);
        //                    continue;
        //                }

        //                fs = File.Create(fileName);
        //                int size = 2048;
        //                byte[] data = new byte[size];
        //                while (true)
        //                {
        //                    size = zipStream.Read(data, 0, data.Length);
        //                    if (size > 0)
        //                        fs.Write(data, 0, data.Length);
        //                    else
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs.Dispose();
        //        }
        //        if (zipStream != null)
        //        {
        //            zipStream.Close();
        //            zipStream.Dispose();
        //        }
        //        if (ent != null)
        //        {
        //            ent = null;
        //        }
        //        GC.Collect();
        //        GC.Collect(1);
        //    }
        //    return result;
        //}

        ///// <summary>   
        ///// 解压功能(解压压缩文件到指定目录)   
        ///// </summary>   
        ///// <param name="fileToUnZip">待解压的文件</param>   
        ///// <param name="zipedFolder">指定解压目标目录</param>   
        ///// <returns>解压结果</returns>   
        //public static bool UnZip(string fileToUnZip, string zipedFolder)
        //{
        //    bool result = UnZip(fileToUnZip, zipedFolder, null);
        //    return result;
        //}

        ////#endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="directoryName"></param>
        public static void UnZip(FileInfo filePath, string directoryName = "")
        {
            if (string.IsNullOrEmpty(directoryName))
            {
                directoryName = Path.GetDirectoryName(filePath.FullName);
            }

            try
            {
                ZipFile.ExtractToDirectory(filePath.FullName, directoryName, Encoding.UTF8);
            }
            catch (InvalidDataException ex)
            {
                if (ex.Message == "End of Central Directory record could not be found.")
                {
                    GzipDecompress(filePath);
                }
            }
        }

        public static void GzipDecompress(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                if (File.Exists(newFileName))
                {
                    return;
                }

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
        }
    }
}
