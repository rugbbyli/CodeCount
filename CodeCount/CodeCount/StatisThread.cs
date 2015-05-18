using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCount
{
    /// <summary>
    /// 文件行计算线程
    /// </summary>
    class StatisThread : UserThread
    {
        private Queue<string> _files;
        private Queue<CountResult> _results;
        private Statistician _statistician;

        private Setting _setting;

        public StatisThread(Queue<string> files, Queue<CountResult> results, Setting set)
        {
            System.Diagnostics.Debug.Assert(files != null && results != null);

            _files        = files;
            _results      = results;
            _setting      = set;

            _statistician = new Statistician();

            _statistician.Init(_setting);
        }

        protected override bool Work()
        {
            string file = string.Empty;
            lock (_files)
            {
                if (_files.Count > 0)
                {
                    file = _files.Dequeue();
                }
            }
            if (string.IsNullOrEmpty(file))
            {
                return false;
            }

            CountResult result;
            using (var stream = new FileStream(file, FileMode.Open))
            {
                result = _statistician.StartNew(stream);
            }


            lock (_results)
            {
                Console.WriteLine(string.Format("{0}:{1}", file, result));
                _results.Enqueue(result);
            }

            return true;
        }
    }
}
