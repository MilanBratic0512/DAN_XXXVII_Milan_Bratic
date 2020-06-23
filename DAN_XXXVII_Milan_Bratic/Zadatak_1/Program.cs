using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        static string path = "../../Routes.txt";
        static Random rnd = new Random();
        static object locker = new object();
        static void Routes()
        {
            lock (locker)
            {
                int[] routes = new int[1000];
                for (int i = 0; i < 1000; i++)
                {
                    routes[i] = rnd.Next(1, 5001);
                }
                using (TextWriter tw = new StreamWriter(path))
                {
                    for (int i = 0; i < routes.Length; i++)
                    {
                        tw.WriteLine(routes[i]);
                    }
                }
                Monitor.Pulse(locker);
            }

        }

       
        static void Main(string[] args)
        {

        }
    }
}
