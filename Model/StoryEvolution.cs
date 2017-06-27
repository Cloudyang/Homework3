using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Model
{
    /// <summary>
    /// 这是在Story基础上扩展，重写InitActor方法 增加Monitor方法
    /// </summary>
    public class StoryEvolution : Story
    {
        /// <summary>
        /// 进阶需求扩展人物
        /// </summary>
        public override void InitActor()
        {
            base.InitActor();
            Actor mrf = new Actor()
            {
                Name = "慕容复"
            };
            mrf.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("出卖王语嫣", action: Start, bgColor: ConsoleColor.Blue, fgColor: ConsoleColor.DarkCyan);
            };
            mrf.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("复燕失败", action: Start, bgColor: ConsoleColor.Blue, fgColor: ConsoleColor.DarkCyan);
            };
            ActorDic.Add(nameof(mrf), mrf);
        }

        /// <summary>
        /// 增加一个监控线程
        /// </summary>
        public void Monitor()
        {
            while (!EnvironmentArgument.cancelTokenSource.IsCancellationRequested)
            {
                var randYear = new Random().Next(0, 10000);
                randYear = 2017;
                if (randYear == DateTime.Now.Year)
                {
                    lock (LogHelper.objLock) //防止出现不正常输出
                    {
                        LogHelper.WriteInfo("天降雷霆灭世，天龙八部的故事就此结束....");
                        EnvironmentArgument.cancelTokenSource.Cancel();
                        break;
                    }
                }
            }
        }
    }
}
