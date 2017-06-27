using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Model
{
    /// <summary>
    /// 人物类
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// 人物名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 人物故事事件
        /// </summary>
        public event Action<Story> ActorStoryEvent;

        /// <summary>
        /// 故事事件发生
        /// </summary>
        public void OnAction(Story story)
        {
            ActorStoryEvent?.Invoke(story);
        }
    }
}
