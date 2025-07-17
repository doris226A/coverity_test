using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

using SagaLib;

namespace SagaMap.Partner
{
        public class AIThread : Singleton<AIThread>
{
    private List<PartnerAI> ais = new List<PartnerAI>();
    private Thread mainThread;
    private ConcurrentQueue<PartnerAI> deleting = new ConcurrentQueue<PartnerAI>();
    private ConcurrentQueue<PartnerAI> adding = new ConcurrentQueue<PartnerAI>();
    private volatile bool stop = false;

    public AIThread()
    {
        mainThread = new Thread(MainLoop);
        mainThread.Name = $"PartnerAIThread({mainThread.ManagedThreadId})";
        SagaLib.Logger.ShowInfo("PartnerAI线程启动：" + mainThread.Name);
        ClientManager.AddThread(mainThread);
        mainThread.Start();
    }

    public void RegisterAI(PartnerAI ai)
    {
        adding.Enqueue(ai);
    }

    public void RemoveAI(PartnerAI ai)
    {
        deleting.Enqueue(ai);
    }

    public int ActiveAI => ais.Count;

        public void RemoveAIAll()
        {
            stop = true;
            mainThread.Join(); // Wait for the thread to finish

            // Replace queues with new instances
            deleting = new ConcurrentQueue<PartnerAI>();
            adding = new ConcurrentQueue<PartnerAI>();
            ais.Clear();

            // Restart the AI thread
            stop = false;
            mainThread = new Thread(MainLoop);
            mainThread.Name = $"PartnerAIThread({mainThread.ManagedThreadId})";
            SagaLib.Logger.ShowInfo("PartnerAI线程重启：" + mainThread.Name);
            ClientManager.AddThread(mainThread);
            mainThread.Start();
        }

        private void MainLoop()
    {
        while (!stop)
        {
            // Process deleting
            while (deleting.TryDequeue(out PartnerAI ai))
            {
                ais.Remove(ai);
            }

            // Process adding
            while (adding.TryDequeue(out PartnerAI ai))
            {
                if (!ais.Contains(ai))
                    ais.Add(ai);
            }

            // Process AI callbacks
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
                        SagaLib.Logger.ShowError(ex);
                    }
                    ai.NextUpdateTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, ai.period);
                }
            }

            // Adjust sleep time based on activity
            Thread.Sleep(ais.Count == 0 ? 500 : 10);
        }
    }
}
}

/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using SagaLib;

namespace SagaMap.Partner
{
    public class AIThread : Singleton<AIThread>
    {
        static List<PartnerAI> ais = new List<PartnerAI>();//线程中的AI
        Thread mainThread;//主线程
        static List<PartnerAI> deleting = new List<PartnerAI>();//删除ai队列
        static List<PartnerAI> adding = new List<PartnerAI>();//增加ai队列
        public AIThread()//构造函数
        {
            mainThread = new Thread(mainLoop);
            mainThread.Name = string.Format("PartnerAIThread({0})", mainThread.ManagedThreadId);
            SagaLib.Logger.ShowInfo("PartnerAI线程启动：" + mainThread.Name);
            ClientManager.AddThread(mainThread);
            mainThread.Start();
        }

        public void RegisterAI(PartnerAI ai)
        {
            lock (adding)
            {
                adding.Add(ai);//如果adding没有被其他线程访问中，则将ai添加入增加队列
            }
        }

        public void RemoveAI(PartnerAI ai)
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
        //ECOKEY 0302卡寵修改測試
        static bool stop = false;
        public void RemoveAIAll()
        {
            stop = true;
            mainThread.Abort();
            while (mainThread.ThreadState != ThreadState.Aborted)
            {
              //  Logger.ShowInfo("還沒結束");
                //当调用Abort方法后，如果thread线程的状态不为Aborted，主线程就一直在这里做循环，直到thread线程的状态变为Aborted为止
                Thread.Sleep(100);
            }
            deleting.Clear();
            adding.Clear();
            ais.Clear();
            ClientManager.RemoveThread(mainThread.Name);
            mainThread = new Thread(mainLoop);
            mainThread.Name = string.Format("PartnerAIThread({0})", mainThread.ManagedThreadId);
            SagaLib.Logger.ShowInfo("PartnerAI线程启动：" + mainThread.Name);
            ClientManager.AddThread(mainThread);
            mainThread.Start();
            stop = false;
        }

        static void mainLoop()
        {
            while (true)
            {
                if (stop) return;//ECOKEY 0302卡寵修改測試
                lock (deleting)//如果deleting没有被其他线程访问中，则遍历删除队列，并移除线程中ai中的要删除的线程，然后清空删除队列
                {
                    foreach (PartnerAI i in deleting)
                    {
                        if (ais.Contains(i))
                            ais.Remove(i);
                    }
                    deleting.Clear();
                }
                lock (adding)
                {
                    foreach (PartnerAI i in adding)
                    {
                        if (!ais.Contains(i))
                            ais.Add(i);
                    }
                    adding.Clear();
                }
                foreach (PartnerAI i in ais)
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
    }
}
*/
