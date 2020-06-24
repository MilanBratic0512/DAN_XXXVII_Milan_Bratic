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
        static SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
        /// <summary>
        /// method for choose and write routes to the file
        /// </summary>
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
                //when routes are choosen Monitor.Puls gives a signal to the Manager
                Monitor.Pulse(locker);
            }

        }

        /// <summary>
        ///  method for pick out best routes
        /// </summary>
        static void Manager()
        {
            lock (locker)
            {
                //wait until routes are written to the file
                Monitor.Wait(locker, rnd.Next(0, 3001));

                //list of all routes
                List<int> allRoutes = new List<int>();
                //all routes divisible by three
                List<int> routesDivisibleByThree = new List<int>();
                //array of strings, fill from the file
                string[] lines = File.ReadAllLines(path);
                //place route to the list
                foreach (var item in lines)
                {
                    allRoutes.Add(int.Parse(item));
                }

                //choose routes divisible by three
                for (int i = 0; i < allRoutes.Count; i++)
                {
                    if (allRoutes[i] % 3 == 0)
                    {
                        routesDivisibleByThree.Add(allRoutes[i]);
                    }
                }
                //sort routes 
                routesDivisibleByThree.Sort();
                //pick out ten best routes
                for (int i = 0; i < bestRoutes.Length; i++)
                {
                    bestRoutes[i] = routesDivisibleByThree[i];
                }
                Console.WriteLine("Routes are selected:");
                //write on the console best routes
                for (int i = 0; i < bestRoutes.Length; i++)
                {
                    Console.WriteLine(bestRoutes[i]);
                }
            }
        }
        /// <summary>
        /// method for truck loading
        /// </summary>
        /// <param name="truckName">represent name of truck</param>
        /// <param name="time">truck loading time</param>
        static void TruckLoading(int truckName, int time)
        {

            semaphore.Wait();
            Console.WriteLine("The truck Truck_{0} loading", truckName);
            Thread.Sleep(time);
            semaphore.Release();

            Console.WriteLine("The truck Truck_{0} is loaded for {1}ms", truckName, time);

        }
        /// <summary>
        /// method for truck unloading
        /// </summary>
        /// <param name="truckName">pass truck name</param>
        /// <param name="time">delivery time</param>
        /// <param name="route">route</param>
        /// <param name="loadingTime">loading time</param>
        static void TruckUnloading(string truckName, int time, int route, int loadingTime)
        {
            Console.WriteLine("The truck Truck_{0} is on its way to route {1}, delivery can be expected for {2}", truckName, route, time);
            if (time > 3000)
            {
                Console.WriteLine("The truck Truck_{0} was canceled. Delivery time is too long", truckName);
                return;
            }
            else
            {
                Thread.Sleep(time);
                Console.WriteLine("The truck Truck_{0} has arrived at its destination, it is unloading time: {1:0.00}ms", truckName, loadingTime / 1.5);
            }


        }
        static void Main(string[] args)
        {
            Thread thread1 = new Thread(Routes);
            Thread thread2 = new Thread(Manager);
            //place loading times into array
            int[] loadingTimes = new int[10];
            thread2.Start();
            thread1.Start();
            thread2.Join();
            thread1.Join();
            for (int i = 0; i < 5; i++)
            {
                int time1 = rnd.Next(500, 5000);
                //since we have five iterations, in this way we ensure that ten threads enterthe method
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
                int time2 = loadingTimes[i];
                int route = bestRoutes[i];
                Thread t = new Thread(() => TruckUnloading(Thread.CurrentThread.Name, time, route, time2));
                t.Name = "" + (i+1);
                t.Start();
            }
            Console.ReadLine();
        }
    }
}
