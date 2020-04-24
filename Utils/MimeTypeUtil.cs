using System;
using System.Collections.Generic;
using System.IO;
using Myrmec;

namespace LogSearchTool.Utils
{
    public static class MimeTypeUtil
    {
        private static Sniffer sniffer;

        private static List<Record> fileTypes = new List<Record>()
        {
            new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,03,04"),
            new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,07,08"),
            new Record("zip,jar,odt,ods,odp,docx,xlsx,pptx,vsdx,apk,aar", "50,4b,05,06"),
            new Record("gz tar.gz", "1F 8B"),
        };

        static MimeTypeUtil()
        {
            sniffer = InitializeSniffer();
        }

        private static Sniffer InitializeSniffer()
        {
            Sniffer sniffer = new Sniffer();

            sniffer.Populate(fileTypes);

            return sniffer;
        }

        public static List<string> GetMimeType(string filePath)
        {
            var fileHead = new byte[20];
            Array.Copy(File.ReadAllBytes(filePath), fileHead, 20);

            return GetMimeType(fileHead);
        }

        public static List<string> GetMimeType(byte[] fileHead)
        {
            return sniffer.Match(fileHead);
        }
    }
}
