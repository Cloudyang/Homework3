using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 日志文件操作类
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// log 配置文件的路径
        /// </summary>
        private static string logPath = EnvironmentArgument.LogPath;
        /// <summary>
        /// 加一个锁
        /// </summary>
        public static readonly object objLock = new object();

        /// <summary>
        /// 设置第一次事件
        /// </summary>
        private static bool FirstStart = true;

        /// <summary>
        /// 输出异常到控制台并写入异常日志
        /// </summary>
        /// <param name="ex">异常信息</param>
        public static void WriteErrorLog(Exception ex)
        {
            //挂起，让信息间隔输出
            Thread.Sleep(700);
            //输出到控制台
            Console.WriteLine($"异常：{ex.Message}");
            //写入日志文件
            _WriteLog(LogEnum.ERROR, ex.ToString());
        }

        public static void WriteErrorLog(string msg)
        {
            //挂起，让信息间隔输出
            Thread.Sleep(700);
            //输出到控制台
            //     Console.WriteLine($"异常：{msg}");
            //写入日志文件
            _WriteLog(LogEnum.ERROR, msg);
        }

        /// <summary>
        /// 输出信息到控制台并写入提示日志
        /// </summary>
        /// <param name="ex">异常信息</param>
        public static void WriteInfo(string message,
            ConsoleColor bgColor = ConsoleColor.Black,
            ConsoleColor fgColor = ConsoleColor.White,
            int timeSecond = 500, Action action = null)
        {
            if (FirstStart)
            {
                lock (objLock)
                {
                    if (FirstStart)
                    {
                        //更新代码：写日志的时候判断FirstStart后
                        WriteInfo(message, timeSecond, bgColor, fgColor);
                        FirstStart = false;
                        action?.Invoke();
                    }
                }
            }
            else
            {
                WriteInfo(message, timeSecond, bgColor, fgColor);
            }
        }

        public static void WriteInfo(string message, int timeSecond, ConsoleColor bgColor, ConsoleColor fgColor)
        {
            lock (objLock)
            {
                if (!EnvironmentArgument.cancelTokenSource.IsCancellationRequested)
                {
                    Console.BackgroundColor = bgColor;
                    Console.ForegroundColor = fgColor;
                    //输出到控制台
                    Console.Write("提示：");
                    var msgs = message.ToCharArray();
                    var delay = timeSecond / (msgs.Length == 0 ? 1 : msgs.Length);
                    foreach (var msg in msgs)
                    {
                        Console.Write(msg);
                        Thread.Sleep(delay);
                    }
                    Console.WriteLine();
                    //写入日志文件
                    _WriteLog(LogEnum.INFO, message);
                }
            }
        }

        #region 内部方法
        /// <summary>
        /// 将日志信息写入文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="message">日志内容</param>
        private static void _WriteLog(LogEnum e, string message)
        {
            string fileName = string.Empty;
            switch (e)
            {
                case LogEnum.INFO:
                    fileName = $"Info_{DateTime.Now.ToString("yyyyMMdd")}.log";
                    //string.Format("Info_{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                    break;
                default:
                    fileName = $"Error_{DateTime.Now.ToString("yyyyMMdd")}.log";
                    //string.Format("Error_{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                    break;
            }
            lock (objLock)
            {
                using (FileStream fs = File.Open(Path.Combine(logPath, fileName), FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(string.Format("{0} --- 线程：{1} --- {2} ", DateTime.Now.ToString().PadRight(20), Thread.CurrentThread.ManagedThreadId.ToString().PadRight(2), message));
                        sw.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        private enum LogEnum
        {
            /// <summary>
            /// 异常日志
            /// </summary>
            ERROR,
            /// <summary>
            /// 信息日志
            /// </summary>
            INFO
        }
        #endregion

    }
}
