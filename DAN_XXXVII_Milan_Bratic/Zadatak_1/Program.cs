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
        static int[] bestRoutes = new int[10];
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

        static void Manager()
        {
            lock (locker)
            {
                Monitor.Wait(locker);

                List<int> allRoutes = new List<int>();
                List<int> routesDivisibleByThree = new List<int>();
                string[] lines = File.ReadAllLines(path);
                foreach (var item in lines)
                {
                    allRoutes.Add(int.Parse(item));
                }

                for (int i = 0; i < allRoutes.Count; i++)
                {
                    if (allRoutes[i] % 3 == 0)
                    {
                        routesDivisibleByThree.Add(allRoutes[i]);
                    }
                }
                routesDivisibleByThree.Sort();
                for (int i = 0; i < bestRoutes.Length; i++)
                {
                    bestRoutes[i] = routesDivisibleByThree[i];
                }
                
            }
        }
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(Routes);
            Thread thread2 = new Thread(Manager);
            thread1.Start();
            thread2.Start();
        }
    }
}
