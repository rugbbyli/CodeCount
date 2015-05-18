using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCount
{
    /// <summary>
    /// 统计结果的线程
    /// </summary>
    class CountThread : UserThread
    {
        private Queue<CountResult> _results;

        public CountResult Result { get; private set; }
        public int TotalCount { get; private set; }

        public CountThread(Queue<CountResult> result)
        {
            System.Diagnostics.Debug.Assert(result != null);

            _results = result;

            Result = new CountResult();
        }

        public override void Start()
        {
            TotalCount = 0;
            base.Start();
        }

        protected override bool Work()
        {
            CountResult result = null;
            lock (_results)
            {
                if (_results.Count > 0)
                {
                    result = _results.Dequeue();
                }
            }
            if (result == null)
            {
                return false;
            }

            Result.Add(result);
            TotalCount++;

            return true;
        }
    }
}
