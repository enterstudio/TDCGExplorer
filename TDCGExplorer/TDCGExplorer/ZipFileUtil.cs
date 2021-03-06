﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiveLib;
using System.IO;
using System.Diagnostics;

namespace TDCGExplorer
{
    public static class ZipFileUtil
    {
        // ファイルを展開する.
        private static bool ExtractZip(IArchive arc, string srcfile, string destpath)
        {
            try
            {
                TDCGExplorer.LastAccessFile = srcfile;
                arc.Open(srcfile);
                if (arc == null) return false;
                foreach (IArchiveEntry entry in arc)
                {
                    if (entry.IsDirectory) continue;
                    if (Path.GetFileName(entry.FileName) == "") continue;
                    using (MemoryStream ms = new MemoryStream((int)entry.Size))
                    {
                        arc.Extract(entry, ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        Directory.CreateDirectory(ReplaceTilda(Path.GetDirectoryName(Path.Combine(destpath, entry.FileName))));

                        string destfilepath = ReplaceTilda(Path.Combine(destpath, entry.FileName));
                        File.Delete(destfilepath);
                        using (Stream fileStream = File.Create(ReplaceTilda(destfilepath)))
                        {
                            BufferedStream bufferedDataStream = new BufferedStream(ms);
                            BufferedStream bufferedFileStream = new BufferedStream(fileStream);
                            CopyStream(bufferedDataStream, bufferedFileStream);

                            bufferedFileStream.Flush();
                            bufferedFileStream.Close();
                            bufferedDataStream.Close();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
            }
            return false;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            input.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[1024];
            int len;
            while ((len = input.Read(buf, 0, buf.Length)) > 0)
            {
                output.Write(buf, 0, len);
            }
        }

        // ZIPファイルを展開する.
        public static bool ExtractZipFile(string srcfile, string destpath)
        {
            switch (Path.GetExtension(srcfile.ToLower()))
            {
                case ".zip":
                    using (IArchive arc = new ZipArchive())
                    {
                        return ExtractZip(arc, srcfile,destpath);
                    }
                case ".rar":
                    using (IArchive arc = new RarArchive())
                    {
                        return ExtractZip(arc, srcfile,destpath);
                    }
                case ".lzh":
                    using (IArchive arc = new LzhArchive())
                    {
                        return ExtractZip(arc, srcfile,destpath);
                    }
                default:
                    using (IArchive arc = new DirectAccessArchive())
                    {
                        return ExtractZip(arc, srcfile, destpath);
                    }
            }
        }

        // オリジナルのファイル名を尊重しつつサマリーを付加する.
        public static string ZipName(string name)
        {
            string node = Path.GetFileNameWithoutExtension(name);
            // ファイル名がnode名だけだった場合.
            if(TDCGExplorer.Arcsnames.ContainsKey(node)==true)
            {
                char[] illegal = { '/', '\\', '*', ':', '?', '<', '>', '\"', '|' };
                ArcsNamesEntry entry = TDCGExplorer.Arcsnames[node];
                string summary = entry.summary;
                foreach (char illeagalchar in illegal)
                {
                    summary = summary.Replace(illeagalchar, ' ');
                }
                node = node + " " + summary;
            }
            return node;
        }

        private static string ReplaceTilda(string input)
        {
            string retval = input.Replace('〜', '～');
            return retval;
        }
    }
}
