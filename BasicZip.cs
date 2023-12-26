using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// BasicZip 的摘要说明
/// </summary>
public class BasicZip
{
    /// 压缩文件夹   
    /// 要打包的文件夹   
    /// 是否删除原文件夹 
    public static string CompressDirectory(string dirPath, bool deleteDir)
    {
        //压缩文件路径
        string pCompressPath = dirPath + ".zip";
        //创建压缩文件
        FileStream pCompressFile = new FileStream(pCompressPath, FileMode.Create);
        using (ZipOutputStream zipoutputstream = new ZipOutputStream(pCompressFile))
        {
            Crc32 crc = new Crc32();
            Dictionary<string, DateTime> fileList = GetAllFies(dirPath);
            foreach (KeyValuePair<string, DateTime> item in fileList)
            {
                FileStream fs = new FileStream(item.Key.ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                // FileStream fs = File.OpenRead(item.Key.ToString());
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(item.Key.Substring(dirPath.Length));
                entry.DateTime = item.Value;
                entry.Size = fs.Length;
                fs.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                zipoutputstream.PutNextEntry(entry);
                zipoutputstream.Write(buffer, 0, buffer.Length);
            }
        }
        if (deleteDir)
        {
            Directory.Delete(dirPath, true);
        }

        return pCompressPath;
    }
    ///    
    /// 获取所有文件   
    ///    
    ///    
    private static Dictionary<string, DateTime> GetAllFies(string dir)
    {
        Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
        DirectoryInfo fileDire = new DirectoryInfo(dir);
        if (!fileDire.Exists)
        {
            throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
        }
        GetAllDirFiles(fileDire, FilesList);
        GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
        return FilesList;
    }
    ///    
    /// 获取一个文件夹下的所有文件夹里的文件   
    ///    
    ///    
    ///    
    private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
    {
        foreach (DirectoryInfo dir in dirs)
        {
            foreach (FileInfo file in dir.GetFiles("."))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
            GetAllDirsFiles(dir.GetDirectories(), filesList);
        }
    }
    ///    
    /// 获取一个文件夹下的文件   
    ///    
    /// 目录名称   
    /// 文件列表HastTable   
    private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
    {
        foreach (FileInfo file in dir.GetFiles())
        {
            filesList.Add(file.FullName, file.LastWriteTime);
        }
    }

    public BasicZip()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
}