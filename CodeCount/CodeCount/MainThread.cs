using System;
using System.Collections.Generic;

namespace CodeCount
{
    /// <summary>
    /// 主线程。负责协调各个线程的工作和维护数据队列。工作过程：
    /// </summary>
    class MainThread : UserThread
    {
        //待统计的文件队列
        private Queue<string> _files;
        //待计算的结果队列
        private Queue<CountResult> _results;

        private FileThread _fileThread;
        private CountThread _countThread;
        private List<StatisThread> _workingThreads;

        public CountResult Result
        {
            get
            {
                return _countThread.Result;
            }
        }

        public MainThread(Setting setting)
        {
            _files          = new Queue<string>();
            _results        = new Queue<CountResult>();
            _workingThreads = new List<StatisThread>();

            _fileThread     = new FileThread(_files, setting);
            _countThread    = new CountThread(_results);

            //使用cpu核心数量一半的计算线程（至少1个）
            var count = Environment.ProcessorCount == 1 ? 1 : Environment.ProcessorCount / 2;

            for (int i = 0; i < count; i++)
            {
                _workingThreads.Add(new StatisThread(_files, _results, setting));
            }
        }

        public override void Start()
        {
            base.Start();

            //启动各个工作线程
            _fileThread.StopWhenIdle = true;
            _fileThread.Start();

            foreach (var thread in _workingThreads)
            {
                thread.StopWhenIdle = false;
                thread.Start();
            }

            _countThread.StopWhenIdle = false;
            _countThread.Start();
            
        }

        /// <summary>
        /// 工作过程描述：
        /// 1，监控各个线程的执行
        /// 2，当磁盘扫描线程完成后，将计算线程设置为“空闲自动退出”；
        /// 3，当计算线程完成后，将统计线程设置为“空闲自动退出”；
        /// 4，当统计线程完成后，主线程退出。
        /// </summary>
        /// <returns></returns>
        protected override bool Work()
        {
            if (_fileThread.State != System.Threading.ThreadState.Stopped)
            {
                return true;
            }
            else
            {
                foreach (var thread in _workingThreads)
                {
                    if (!thread.StopWhenIdle)
                    {
                        thread.StopWhenIdle = true;
                    }
                }
            }

            foreach (var thread in _workingThreads)
            {
                if (thread.State != System.Threading.ThreadState.Stopped)
                {
                    return true;
                }
            }

            if (!_countThread.StopWhenIdle)
            {
                _countThread.StopWhenIdle = true;
            }

            if (_countThread.State != System.Threading.ThreadState.Stopped)
            {
                return true;
            }
            
            return false;
        }
    }
}
