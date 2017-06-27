using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Common;

namespace Model
{
    /// <summary>
    /// 剧情类
    /// </summary>
    public class Story
    {
        /// <summary>
        /// 剧情名称
        /// </summary>
        public string Name { get; set; } = "天龙八部";

        /// <summary>
        /// 剧情结束语
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 记录所有角色
        /// </summary>
        public static Dictionary<string, Actor> ActorDic = new Dictionary<string, Actor>();

        /// <summary>
        /// 初始化剧情人物
        /// </summary>
        public virtual void InitActor()
        {
            #region 乔峰
            Actor qiaofeng = new Actor()
            {
                Name = "乔峰"
            };
            qiaofeng.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("丐帮帮主", action: Start);
            };
            qiaofeng.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("契丹人", action: Start);
            };
            qiaofeng.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("南院大王", action: Start);
            };
            qiaofeng.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("挂印离开", action: Start);
            };
            #endregion
            ActorDic.Add(nameof(qiaofeng), qiaofeng);

            #region 虚竹
            Actor xuzhu = new Actor()
            {
                Name = "虚竹"
            };
            xuzhu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("小和尚", action: Start);
            };
            xuzhu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("逍遥掌门", action: Start);
            };
            xuzhu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("灵鹫宫宫主", action: Start);
            };
            xuzhu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("西夏驸马", action: Start);
            };
            #endregion
            ActorDic.Add(nameof(xuzhu), xuzhu);

            #region 段誉
            Actor duanyu = new Actor()
            {
                Name = "段誉"
            };
            duanyu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("钟灵儿", action: Start);
            };
            duanyu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("木婉清", action: Start);
            };
            duanyu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("王语嫣", action: Start);
            };
            duanyu.ActorStoryEvent += (story) =>
            {
                LogHelper.WriteInfo("大理国王", action: Start);
            };
            #endregion
            ActorDic.Add(nameof(duanyu), duanyu);
        }

        public Story()
        {
            InitActor();
        }

        /// <summary>
        /// 任何一个人完成第一件事后，需要执行
        /// </summary>
        public virtual void Start()
        {
            LogHelper.WriteInfo($"{Name}就此拉开序幕。。。。");
        }

        /// <summary>
        /// 以上的任何一个人物线完成了，需要执行一件事
        /// </summary>
        /// <param name="actor"></param>
        public virtual void AnyFinish(Actor actor)
        {
            LogHelper.WriteInfo($"{actor.Name}已经做好准备啦。。。。");
        }

        /// <summary>
        /// 以上的任何一个人物线完成了，需要执行一件事
        /// </summary>
        /// <param name="actor"></param>
        public virtual void AnyFinish()
        {
            LogHelper.WriteInfo($"某某已经做好准备啦。。。。");
        }

        /// <summary>
        /// 等3条人物线全部完成后，执行
        /// </summary>
        public virtual void End()
        {
            LogHelper.WriteInfo("中原群雄大战辽兵，忠义两难一死谢天");
        }
    }
}
