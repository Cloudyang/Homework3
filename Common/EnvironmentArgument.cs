using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Threading;

namespace Common
{
    public class EnvironmentArgument
    {
        /// <summary>
        /// 引入公有多线程控制信号量
        /// </summary>
        public static CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 日志存放目录
        /// </summary>
        public static string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["LogPath"]);

        /// <summary>
        /// XML存放目录
        /// </summary>
        public static string XmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["XmlPath"]);

        /// <summary>
        /// Json存放目录
        /// </summary>
        public static string JsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["JsonPath"]);

        static EnvironmentArgument()
        {
            Directory.CreateDirectory(LogPath);
            Directory.CreateDirectory(XmlPath);
            Directory.CreateDirectory(JsonPath);

        }
    }
}
