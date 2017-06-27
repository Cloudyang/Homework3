using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model;
using Common;

namespace Homework3
{
    class Program
    {
        static void Main(string[] args)
        {
            ///定义：天龙八部
          //  var tlbb = new Story();

            ///扩展：增加角色及事件：慕容复
            var tlbb = new StoryEvolution();


            Stopwatch watch = new Stopwatch();
            watch.Start();

            //#region Task任务方式实现
            List<Task> taskList = new List<Task>();
            try
            {

                ///增加一个监控线程
                Task.Factory.StartNew(() => tlbb.Monitor());

                foreach (var actor in Story.ActorDic.Values)
                {
                    Task task = Task.Factory.StartNew((a) => actor.OnAction(tlbb), actor);
                    taskList.Add(task);
                }

                ///需要执行一件事是“某某已经做好准备啦。。。
                taskList.Add(Task.Factory.ContinueWhenAny(taskList.ToArray(), t =>
                {
                    if (t.Status != TaskStatus.Canceled)
                    {
                        tlbb.AnyFinish(t.AsyncState as Actor);
                    }
                }));

                taskList.Add(Task.Factory.ContinueWhenAll(taskList.ToArray(), ts =>
                {
                    if (!ts.Any(t => t.Status == TaskStatus.Canceled))
                    {
                        tlbb.End();
                        ///结束监控线程
                        EnvironmentArgument.cancelTokenSource.Cancel();
                    }
                }));

                Task.WaitAll(taskList.ToArray());
            }
            catch (AggregateException aex)
            {
                foreach (var item in aex.InnerExceptions)
                {
                    LogHelper.WriteErrorLog(item.Message);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
            }
            //#endregion

            watch.Stop();

            Console.WriteLine($"统计出来整个天龙八部的故事花了{watch.ElapsedMilliseconds}时间");
            Console.ReadKey();
        }
    }
}
