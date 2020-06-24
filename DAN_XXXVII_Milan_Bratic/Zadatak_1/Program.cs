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
        static Thread[] trucks = new Thread[10];
        static Dictionary<string, int> TrucksAndLoadingTime = new Dictionary<string, int>();
        static Dictionary<string, int> TrucksAndDeliveryTime = new Dictionary<string, int>();
        static SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
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
                Monitor.Wait(locker, rnd.Next(0, 3001));

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
                Console.WriteLine("Routes are selected:");
                for (int i = 0; i < bestRoutes.Length; i++)
                {
                    Console.WriteLine(bestRoutes[i]);
                }
            }
        }

        static void TruckLoading(int i, int time)
        {

            semaphore.Wait();
            Console.WriteLine("The truck Truck_{0} loading", i);
            Thread.Sleep(time);



            semaphore.Release();

            Console.WriteLine("The truck Truck_{0} is loaded for {1}ms", i, time);

        }
        static void TruckUnloading(int i, int time, int route, int loadingTime)
        {
            Console.WriteLine("The truck Truck_{0} is on its way to route {1}, delivery can be expected for {2}", i, route, time);
            if (time > 3000)
            {
                Console.WriteLine("The truck Truck_{0} was canceled. Delivery time is too long", i);
                return;
            }
            else
            {
                Thread.Sleep(time);
                Console.WriteLine("The truck Truck_{0} has arrived at its destination, it is unloading time: {1}", i, loadingTime / 1.5);
            }


        }
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(Routes);
            Thread thread2 = new Thread(Manager);
            int[] loadingTimes = new int[10];
            thread2.Start();
            thread1.Start();
            thread2.Join();
            thread1.Join();
            for (int i = 0; i < 5; i++)
            {
                int time1 = rnd.Next(500, 5000);
                loadingTimes[2 * i] = time1;
                int time2 = rnd.Next(500, 5000);
                loadingTimes[2 * i + 1] = time2;
                Thread t1 = new Thread(() => TruckLoading(2 * i + 1, time1));
                Thread t2 = new Thread(() => TruckLoading(2 * i + 2, time2));
                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
            }

            for (int i = 0; i < 10; i++)
            {
                int time = rnd.Next(500, 5000);
                Thread t = new Thread(() => TruckUnloading(i + 1, time, bestRoutes[i], loadingTimes[i]));
                t.Start();
                t.Join();
            }
            Console.ReadLine();
        }
    }
}
