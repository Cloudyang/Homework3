using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model;
using Common;
using Common.ThreadExtend;

namespace Homework3
{
    class Program
    {
        static void Main(string[] args)
        {
          //  第一次生成Json配置使用
          //  InitWriteJson();
            #region 测试多线程带返回值演练
            Thread thread = new Thread(() =>
              {
                  Console.WriteLine($"开始测试多线程演练工作——线程ID：{Thread.CurrentThread.ManagedThreadId}");
              });

            var callReturn = thread.WithReturn((s) =>
            {
                Thread.Sleep(1000);
                return $"返回结果状态：{s}";
            }, "测试多线程带返回值");

            Thread.Sleep(500);
            Console.WriteLine($"Thread.Sleep模拟主线程还在工作1——线程ID：{Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(200);
            Console.WriteLine($"Thread.Sleep模拟主线程还在工作2——线程ID：{Thread.CurrentThread.ManagedThreadId}");

            var sReturn = string.Empty;
            sReturn = callReturn.Invoke();
            Console.WriteLine($"主线程还在工作——线程ID：{Thread.CurrentThread.ManagedThreadId} 返回结果：{sReturn}");

            #endregion

            #region 天龙八部 ---测试代码运行
            TestStoryDemo();
            #endregion


            Console.ReadKey();
        }

        private static void InitWriteJson()
        {
            List<ActorInfo> actorInfos = new List<ActorInfo> {
                new ActorInfo
                {
                    Name = "丁春秋",
                    Color = ConsoleColor.Green,
                    StoryContent = new List<string>
                     {
                        "后背叛师门",
                        "化功大法",
                        "生死符"
                     }
                },new ActorInfo
                {
                    Name = "扫地僧",
                    Color = ConsoleColor.Blue,
                    StoryContent = new List<string>
                     {
                        "无形高墙",
                        "皈依佛门",
                     }
                },new ActorInfo
                {
                    Name = "鸠摩智",
                    Color = ConsoleColor.Green,
                    StoryContent = new List<string>
                     {
                        "拈花指",
                        "小无相功",
                        "大金刚拳",
                        "钻研佛经"
                     }
                }
            };
            var filePath = System.IO.Path.Combine(EnvironmentArgument.JsonPath, "TLBB.json");
            JsonHelper.WriteJsonFile(filePath, actorInfos);
        }

        private static void TestStoryDemo()
        {
            ///定义：天龙八部
            //  var tlbb = new Story();

            ///扩展：增加角色及事件：慕容复
            var tlbb = new StoryEvolution();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            #region Task任务方式实现
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
                taskList.Add(Task.Factory.ContinueWhenAny(taskList.ToArray(), task =>
                {
                    if (task.Status != TaskStatus.Canceled)
                    {
                        tlbb.AnyFinish(task.AsyncState as Actor);
                    }
                }));

                taskList.Add(Task.Factory.ContinueWhenAll(taskList.ToArray(), tasks =>
                {
                    if (!tasks.Any(t => t.Status == TaskStatus.Canceled))
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
            #endregion

            watch.Stop();

            Console.WriteLine($"统计出来整个天龙八部的故事花了{watch.ElapsedMilliseconds}时间");
        }
    }
}
