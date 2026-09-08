using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Framework.Util
{
    class FileUtil
    {
        public static bool IsExists(string path)
        {
            FileInfo file = new FileInfo(path);
            return file.Exists;
        }

        public static void WriteFile(string path, byte[] data)
        {
            // 规范化路径
            path = path.Replace("\\", "/");
            // 接取文件夹路径
            string dir = path.Substring(0, path.LastIndexOf("/"));
            // 判断是否存在这个文件夹 没有就创建
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            FileInfo file = new FileInfo(path);

            // 如果文件存在，就删掉
            if(file.Exists)
            {
                file.Delete();
            }

            try
            {
                // using是一个语法糖
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
            } catch(IOException e)
            {
                AppLog.LogError("FileUtil IO", $"写入失败: {e.Message}");
            }

        }
    }
}
