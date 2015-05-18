using System;
using System.Threading;

namespace CodeCount
{
    /// <summary>
    /// 各个工作线程的基类
    /// </summary>
    class UserThread
    {
        /// <summary>
        /// 当线程空闲下来时是否自动关闭。
        /// </summary>
        public bool StopWhenIdle { get; set; }

        public ThreadState State { get; protected set; }

        public virtual void Start()
        {
            State = ThreadState.Unstarted;

            ThreadPool.QueueUserWorkItem(ThreadLoop);
        }

        /// <summary>
        /// 线程主循环逻辑
        /// </summary>
        protected virtual void ThreadLoop(object arg)
        {
            try
            {
                State = ThreadState.Running;
                while (State == ThreadState.Running)
                {
                    if (!Work())
                    {
                        if (StopWhenIdle)
                        {
                            State = ThreadState.Stopped;
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("error:{0}", e.Message));
            }
        }

        /// <summary>
        /// 线程工作逻辑。如果线程空闲了，返回false，否则返回true。
        /// </summary>
        /// <returns>线程是否还在工作</returns>
        protected virtual bool Work()
        {
            return false;
        }
    }
}
