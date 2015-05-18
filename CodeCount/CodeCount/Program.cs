using System;

namespace CodeCount
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MainWithoutArgs();
            }
            else
            {
                try
                {
                    Work(Setting.Parse(args));
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("error:{0}", e.Message));
                }
            }
        }

        static void MainWithoutArgs()
        {
            while (true)
            {
                string path;
                do
                {
                    Console.WriteLine("please input working path:");
                    path = Console.ReadLine();
                } while (!System.IO.Directory.Exists(path));

                string lang;
                do
                {
                    Console.WriteLine("please select language(C/C++/C#/JAVA):");
                    lang = Console.ReadLine().ToUpper();
                } while (!LanguageSetting.Setting.ContainsKey(lang));

                var set = LanguageSetting.Setting[lang];
                set.Path = path;

                try
                {
                    Work(set);
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("error:{0}", e.Message));
                }
            }
        }

        static void Work(Setting set)
        {
            MainThread thread = new MainThread(set);
            thread.StopWhenIdle = true;
            thread.Start();

            while (thread.State != System.Threading.ThreadState.Stopped)
            {
                System.Threading.Thread.Sleep(0);
            }

            Console.WriteLine(string.Format("Result:{0}", thread.Result));
        }
    }
}
