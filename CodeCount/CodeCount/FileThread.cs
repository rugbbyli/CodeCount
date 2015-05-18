using System;
using System.Collections.Generic;
using System.IO;

namespace CodeCount
{
    /// <summary>
    /// 获取磁盘目录下全部文件的线程
    /// </summary>
    class FileThread : UserThread
    {
        private Queue<string> _files;
        private Setting _setting;

        public FileThread(Queue<String> files, Setting setting)
        {
            System.Diagnostics.Debug.Assert(files != null && setting != null);

            _files = files;
            _setting = setting;
        }

        protected override bool Work()
        {
            foreach (var ext in _setting.ExtArray)
            {
                EnumFiles(_setting.Path, ext);
            }
            return false;
        }

        private void EnumFiles(string folder, string pattern)
        {
            try
            {
                AddToFileQueue(Directory.EnumerateFiles(folder, pattern));

                foreach (var subfolder in Directory.EnumerateDirectories(folder))
                {
                    EnumFiles(subfolder, pattern);
                }
            }
            catch { }
        }

        private void AddToFileQueue(IEnumerable<string> files)
        {
            lock (_files)
            {
                foreach (var file in files)
                {
                    _files.Enqueue(file);
                }
            }
        }
    }
}
