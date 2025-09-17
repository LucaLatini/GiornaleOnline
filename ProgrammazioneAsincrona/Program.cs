using System.Diagnostics;

namespace ProgrammazioneAsincrona
{
    internal class Program
    {
        static readonly Stopwatch timer = new Stopwatch();
         static async Task Main(string[] args)
        {
            timer.Start();
            Console.WriteLine("Main inizio :" + timer.Elapsed.ToString());
            Task task1 = Task1Async();
             Task task2 = Task2Async();


            await Task.WhenAll(task1,task2);
            Console.WriteLine("Main fine :" + timer.Elapsed.ToString());
            timer.Stop();


        }

        public static async Task FaiQualcosaAsync()
        {

        }

        public static async Task<int> FaiQualcosaETornaRisultatoAsync()
        {
            return 1;

        }

        private static void Task1()
        {
            Console.WriteLine("Task1 inizio :" + timer.Elapsed.ToString());
            Thread.Sleep(2000);
            Console.WriteLine("Task1 fine :" + timer.Elapsed.ToString());
        }

        private static void Task2()
        {
            Console.WriteLine("Task2 inizio :" + timer.Elapsed.ToString());
            Thread.Sleep(3000);
            Console.WriteLine("Task2 fine :" + timer.Elapsed.ToString());
        }

        private static async Task Task1Async()
        {
            Console.WriteLine(" async Task1 inizio :" + timer.Elapsed.ToString());
            await Task.Delay(2000);
            Console.WriteLine(" async Task1 fine :" + timer.Elapsed.ToString());
        }
        private static async Task Task2Async()
        {
            Console.WriteLine(" async Task2 inizio :" + timer.Elapsed.ToString());
            await Task.Delay(3000);
            Console.WriteLine(" async Task2 fine :" + timer.Elapsed.ToString());
        }


    }
}