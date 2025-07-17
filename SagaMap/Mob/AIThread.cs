using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Collections.Concurrent;

using SagaLib;

/*namespace SagaMap.Mob
{
    public class AIThread : Singleton<AIThread>
    {
        static ConcurrentBag<MobAI> ais = new ConcurrentBag<MobAI>();
        static ConcurrentBag<MobAI> deleting = new ConcurrentBag<MobAI>();
        static ConcurrentBag<MobAI> adding = new ConcurrentBag<MobAI>();
        Thread mainThread;

        public AIThread()
        {
            mainThread = new Thread(mainLoop);
            mainThread.Name = string.Format("MobAIThread({0})", mainThread.ManagedThreadId);
            SagaLib.Logger.ShowInfo("MobAI线程启动：" + mainThread.Name);
            ClientManager.AddThread(mainThread);
            mainThread.Start();
        }

        public void RegisterAI(MobAI ai)
        {
            adding.Add(ai);
        }

        public void RemoveAI(MobAI ai)
        {
            deleting.Add(ai);
        }

        public int ActiveAI
        {
            get
            {
                return ais.Count;
            }
        }

        static void mainLoop()
        {
            try
            {
                while (true)
                {
                    // 处理删除
                    var currentDeleting = new List<MobAI>(deleting);
                    deleting = new ConcurrentBag<MobAI>(); // Clear by creating a new instance

                    foreach (var ai in currentDeleting)
                    {
                        ais = new ConcurrentBag<MobAI>(ais.Where(a => a != ai)); // Remove ai from ais
                    }

                    // 处理添加
                    var currentAdding = new List<MobAI>(adding);
                    adding = new ConcurrentBag<MobAI>(); // Clear by creating a new instance

                    foreach (var ai in currentAdding)
                    {
                        ais.Add(ai);
                    }

                    // 处理 AI 更新
                    foreach (var ai in ais)
                    {
                        if (!ai.Activated)
                            continue;
                        if (DateTime.Now > ai.NextUpdateTime)
                        {
                            try
                            {
                                ai.CallBack(null);
                            }
                            catch (Exception ex)
                            {
                                Logger.ShowError(ex);
                            }
                            ai.NextUpdateTime = DateTime.Now + TimeSpan.FromMilliseconds(ai.period);
                        }
                    }

                    Thread.Sleep(10); // 统一的短休眠时间
                }
            }
            catch (Exception ex)
            {
                SagaLib.Logger.ShowError(ex);
            }
        }
    }
}*/
namespace SagaMap.Mob
{
    public class AIThread : Singleton<AIThread>
    {
       // static ConcurrentBag<MobAI> ais = new ConcurrentBag<MobAI>();
       // static ConcurrentBag<MobAI> deleting = new ConcurrentBag<MobAI>();
       // static ConcurrentBag<MobAI> adding = new ConcurrentBag<MobAI>();

          static List<MobAI> ais = new List<MobAI>();//线程中的AI
        Thread mainThread;//主线程
        static List<MobAI> deleting = new List<MobAI>();//删除ai队列
        static List<MobAI> adding = new List<MobAI>();//增加ai队列
        public AIThread()//构造函数
        {
            mainThread = new Thread(mainLoop);
            mainThread.Name = string.Format("MobAIThread({0})", mainThread.ManagedThreadId);
            SagaLib.Logger.ShowInfo("MobAI线程启动：" +mainThread.Name);
            ClientManager.AddThread(mainThread);
            mainThread.Start();
        }

        public void RegisterAI(MobAI ai)
        {
            lock (adding)
            {
                adding.Add(ai);//如果adding没有被其他线程访问中，则将ai添加入增加队列
            }
        }

        public void RemoveAI(MobAI ai)
        {
            lock (deleting)
            {
                deleting.Add(ai);//如果deleting没有被其他线程访问中，则将ai添加入删除队列
            }
        }

        public int ActiveAI
        {
            get
            {
                return ais.Count;//返回线程中ai的数量
            }
        }

        static void mainLoop()
        {
            try
            {
                while (true)
                {
                    lock (deleting)//如果deleting没有被其他线程访问中，则遍历删除队列，并移除线程中ai中的要删除的线程，然后清空删除队列
                    {
                        foreach (MobAI i in deleting)
                        {
                            if (ais.Contains(i))
                                ais.Remove(i);
                        }
                        deleting.Clear();
                    }
                    lock (adding)
                    {
                        foreach (MobAI i in adding)
                        {
                            if (!ais.Contains(i))
                                ais.Add(i);
                        }
                        adding.Clear();
                    }
                    foreach (MobAI i in ais)
                    {
                        if (!i.Activated)
                            continue;
                        if (DateTime.Now > i.NextUpdateTime)
                        {
                            //ClientManager.EnterCriticalArea();
                            try
                            {
                                i.CallBack(null);
                            }
                            catch (Exception ex)
                            {
                                Logger.ShowError(ex);
                            }
                            i.NextUpdateTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, i.period);
                            //ClientManager.LeaveCriticalArea();
                        }
                    }
                    if (ais.Count == 0)
                        Thread.Sleep(500);
                    else
                        Thread.Sleep(10);
                }
            }
            catch(Exception ex)
            {
                SagaLib.Logger.ShowError(ex);
            }
        }
    }
}
