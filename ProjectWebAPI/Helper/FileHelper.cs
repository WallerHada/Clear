using System;
using System.IO;
using System.Linq;

namespace ProjectWebAPI.Helper
{
    public class FileHelper
    {
        #region 根据文件大小获取指定前缀的可用文件名
        /// <summary>
        /// 根据文件大小获取指定前缀的可用文件名
        /// </summary>
        /// <param name="folderPath">文件夹</param>
        /// <param name="prefix">文件前缀</param>
        /// <param name="size">文件大小(1m)</param>
        /// <param name="ext">文件后缀(.log)</param>
        /// <returns>可用文件名</returns>
        public static string GetAvailableFileWithPrefixOrderSize(string folderPath, string prefix, int size = 1 * 1024 * 1024, string ext = ".log")
        {
            var allFiles = new DirectoryInfo(folderPath);
            var selectFiles = allFiles.GetFiles().Where(fi => fi.Name.ToLower().Contains(prefix.ToLower()) && fi.Extension.ToLower() == ext.ToLower() && fi.Length < size).OrderByDescending(d => d.Name).ToList();

            if (selectFiles.Count > 0)
            {
                return selectFiles.FirstOrDefault().FullName;
            }

            return Path.Combine(folderPath, $@"{prefix}_{DateTime.Now.DateToTimeStamp()}.log");
        }
        public static string GetAvailableFileNameWithPrefixOrderSize(string _contentRoot, string prefix, int size = 1 * 1024 * 1024, string ext = ".log")
        {
            var folderPath = Path.Combine(_contentRoot, "Log");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var allFiles = new DirectoryInfo(folderPath);
            var selectFiles = allFiles.GetFiles().Where(fi => fi.Name.ToLower().Contains(prefix.ToLower()) && fi.Extension.ToLower() == ext.ToLower() && fi.Length < size).OrderByDescending(d => d.Name).ToList();

            if (selectFiles.Count > 0)
            {
                return selectFiles.FirstOrDefault().Name.Replace(".log", "");
            }

            return $@"{prefix}_{DateTime.Now.DateToTimeStamp()}";
        }
        #endregion
    }
}
