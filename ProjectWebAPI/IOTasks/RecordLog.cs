using ProjectWebAPI.Helper;
using System;
using System.IO;
using System.Threading;

namespace ProjectWebAPI.IOTasks
{
    public class RecordLog
    {
#if false
        class name DirAppend
        using (StreamWriter w = File.AppendText("log.txt"))
        {
            Log("Test1", w);
            Log("Test2", w);
        }
#endif

        static ReaderWriterLockSlim LogWriteLock = new();
        static int WritedCount = 0;
        static int FailedCount = 0;
        static string _contentPath = string.Empty;

        public RecordLog(string contentPath)
        {
            _contentPath = contentPath;
        }

        public static void ErrorLog(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static void HighfrequencyLog(string prefix, string[] dataParas, bool IsHeader = true)
        {
            try
            {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();

                var folderPath = Path.Combine(_contentPath, "Log");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                //string logFilePath = Path.Combine(path, $@"{filename}.log");
                var logFilePath = FileHelper.GetAvailableFileWithPrefixOrderSize(folderPath, prefix);

                var now = DateTime.Now;
                string logContent = String.Join("\r\n", dataParas);
                if (IsHeader)
                {
                    logContent = (
                       "--------------------------------\r\n" +
                       DateTime.Now + "|\r\n" +
                       String.Join("\r\n", dataParas) + "\r\n"
                       );
                }

                //if (logContent.IsNotEmptyOrNull() && logContent.Length > 500)
                //{
                //    logContent = logContent.Substring(0, 500) + "\r\n";
                //}

                File.AppendAllText(logFilePath, logContent);
                WritedCount++;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                FailedCount++;
            }
            finally
            {
                //退出写入模式，释放资源占用
                //注意：一次请求对应一次释放
                //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}
