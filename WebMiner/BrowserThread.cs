using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Awesomium.Core;
using ThreadState = System.Threading.ThreadState;

namespace WebMiner
{
    static class BrowserThread
    {
        private static readonly ConcurrentQueue<Tuple<Guid, Func<JSValue>>> CommandQueue;
        private static readonly ConcurrentDictionary<Guid,JSValue> Results;

        static BrowserThread()
        {
            CommandQueue= new ConcurrentQueue<Tuple<Guid, Func<JSValue>>>();
            Results = new ConcurrentDictionary<Guid, JSValue>();
            var thread = new Thread(BgBrowserLoop);
            thread.Name = "BgBrowserThread";
            thread.IsBackground = true;
            thread.Start();
        }

        private static void BgBrowserLoop()
        {
            WebCore.Initialize(WebConfig.Default, true);
            while (Thread.CurrentThread.ThreadState != ThreadState.AbortRequested)
            {
                Tuple<Guid, Func<JSValue>> cCommand;
                if (CommandQueue.TryDequeue(out cCommand))
                {
                    try
                    {
                        Results.TryAdd(cCommand.Item1, cCommand.Item2());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception in browserthread: " + ex.Message);
                    }
                }
                else
                {
                    WebCore.Update();
                    Thread.Sleep(10);
                }
            }
        }

        public static JSValue ExecuteFunction(Func<JSValue> pDelegate)
        {
            if (Thread.CurrentThread.Name == "BgBrowserThread")
                return pDelegate();
                
            var id = Guid.NewGuid();
            CommandQueue.Enqueue(new Tuple<Guid, Func<JSValue>>(id,pDelegate));
            JSValue retVal;
            while (!Results.TryRemove(id, out retVal))
            {
                Thread.Sleep(10);
            }
            return retVal;
        }
            
        public static void ExecuteAction(Action pAction)
        {
            ExecuteFunction(() =>
                {
                    pAction();
                    return true;
                });
        }
    }
}