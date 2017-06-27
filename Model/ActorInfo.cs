using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// 人物实体类
    /// </summary>
    public class ActorInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ConsoleColor Color { get; set; }
        /// <summary>
        /// 故事内容
        /// </summary>
        public List<string> StoryContent { get; set; }
    }
}
