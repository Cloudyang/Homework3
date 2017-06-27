using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Common
{
    public class JsonHelper
    {
        private static readonly object objLock = new object();
        #region Json
        /// <summary>
        /// JsonConvert.SerializeObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// JsonConvert.DeserializeObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T ToObject<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        #endregion Json

        #region 读写文件方法
        /// <summary>
        /// 读取Json文件生成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T ReadJsonFile<T>(string filePath)
        {
            T t = default(T);
            lock (objLock)
            {
                var content = string.Empty;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        content = sr.ReadToEnd();
                    }
                }
                t = ToObject<T>(content);
            }
            return t;
        }
        /// <summary>
        /// 写入join文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="sourceObj"></param>
        public static void WriteJsonFile<T>(string filePath, T sourceObj)
        {
            var content = string.Empty;
            lock (objLock)
            {
                content = ToJson(sourceObj);
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(content);
                        sw.Flush();
                    }
                }
            }
        }
        #endregion
    }
}
