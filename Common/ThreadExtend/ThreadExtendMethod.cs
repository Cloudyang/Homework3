using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Common.ThreadExtend
{
    public static class ThreadExtend
    {
        #region 封装一个使用ThreadPool的带返回值的
        /// <summary>
        /// 通过ManualResetEvent.Wait方法来同步线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thread"></param>
        /// <param name="call"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Func<T> WithReturn<T>(this Thread thread, Func<object, T> call, object state)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            T t = default(T);
            ThreadPool.QueueUserWorkItem((s) =>
            {
                t = call(s);
                mre.Set();
            }, state);
            thread.Start();

            return new Func<T>(() =>
            {
                mre.WaitOne();
                return t;
            });
        }

        /// <summary>
        /// 通过Thread.Join同步到调用线程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="call"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Func<T> WithReturn<T>(Func<object, T> call, object state)
        {
            T t = default(T);

            var thread = new Thread((s) =>
            {
                t = call(s);
            });
            thread.Start(state);

            return new Func<T>(() =>
            {
                thread.Join();
                return t;
            });
        }

        #endregion

        #region 如下代码学习第八期625-HomeWork3
        #region   Delegate   委托的异步调用
        private static object _lock = new object();
        /// <summary>
        /// Delegate实现ContinueWhenAny
        /// </summary>
        /// <param name="actionArray">委托数组</param>
        /// <param name="callback">回调</param>
        private static void DalegateContinueWhenAny(Action[] actionArray, Action callback)
        {
            bool isCompleted = false;
            foreach (var action in actionArray)
            {
                action.BeginInvoke(t =>
                {
                    if (!isCompleted)
                    {
                        lock (_lock)
                        {
                            if (!isCompleted)
                            {
                                isCompleted = true;
                                callback.Invoke();

                            }
                        }
                    }
                }, null);
            }
        }

        private static void DelegateContinueWhenAll(Action[] actionArray, Action callback)
        {
            int isCompleted = 0;
            int actionCount = actionArray.Length;
            foreach (var action in actionArray)
            {
                action.BeginInvoke(t =>
                {
                    lock (_lock)
                    {
                        isCompleted++;
                    }
                    if (isCompleted == actionCount)
                    {
                        callback.Invoke();
                    }
                }, null);
            }
        }


        #endregion

        #region   Thread 
        /// <summary>
        /// ThreadContinueWhenAny
        /// </summary>
        /// <param name="actionArray"></param>
        /// <param name="callback"></param>
        public static void ThreadContinueWhenAny(Action[] actionArray, Action callback)
        {
            List<Thread> threadList = new List<Thread>();
            foreach (var action in actionArray)
            {
                Thread thread = new Thread(() => action.Invoke());
                thread.Start();
                threadList.Add(thread);
            }
            Thread th = new Thread(t =>
            {
                while (true)
                {
                    if (threadList.Count(x => x.ThreadState == ThreadState.Stopped) > 0)
                    {
                        callback();
                        break;
                    }
                    Thread.Sleep(100);
                }
            });
            th.Start();
        }
        /// <summary>
        /// ThreadContinueWhenAll 
        /// </summary>
        /// <param name="actionArray"></param>
        /// <param name="callback"></param>
        public static void ThreadContinueWhenAll(Action[] actionArray, Action callback)
        {
            List<Thread> threadList = new List<Thread>();
            ParameterizedThreadStart method = null;
            foreach (var action in actionArray)
            {
                method = t => action();
                Thread thread = new Thread(method);
                thread.Start();
                threadList.Add(thread);
            }

            Thread th = new Thread(t =>
            {
                while (true)
                {
                    if (threadList.Count(x => x.ThreadState != ThreadState.Stopped) == 0)
                    {
                        callback();  //update1   外面已经是异步了 没必要再开一个线程
                        break;
                    }
                    Thread.Sleep(100);
                }
            });
            th.Start();
        }

        #endregion

        #region   ThreadPool
        /// <summary>
        /// ThreadPoolContinueWhenAny
        /// </summary>
        /// <param name="actionArray"></param>
        /// <param name="callback"></param>
        public static void ThreadPoolContinueWhenAny(Action[] actionArray, Action callback)
        {
            List<ManualResetEvent> manualResetEventList = new List<ManualResetEvent>();
            foreach (var action in actionArray)
            {
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                manualResetEventList.Add(manualResetEvent);
                ThreadPool.QueueUserWorkItem(new WaitCallback(t =>
                {
                    action();
                    manualResetEvent.Set();
                }));
            }


            //update1 不要卡主线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(t =>
            {
                WaitHandle.WaitAny(manualResetEventList.ToArray());
                callback();
            }));
        }

        /// <summary>
        /// ThreadPoolContinueWhenAll
        /// </summary>
        /// <param name="actionArray"></param>
        /// <param name="callback"></param>
        public static void ThreadPoolContinueWhenAll(Action[] actionArray, Action callback)
        {
            List<ManualResetEvent> manualResetEventList = new List<ManualResetEvent>();
            foreach (var action in actionArray)
            {
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                manualResetEventList.Add(manualResetEvent);
                ThreadPool.QueueUserWorkItem(new WaitCallback(t =>
                {
                    action();
                    manualResetEvent.Set();
                }));
            }
            //if (WaitHandle.WaitAll(manualResetEventList.ToArray()))
            //{
            //    ThreadPool.QueueUserWorkItem(new WaitCallback(x => callback()));
            //}

            //update1 不要卡主线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(t =>
            {
                WaitHandle.WaitAll(manualResetEventList.ToArray());
                callback();
            }));

            #endregion
            #endregion
        }
    }
}
