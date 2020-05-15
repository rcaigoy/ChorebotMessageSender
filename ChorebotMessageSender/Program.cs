using System;
using System.Threading;
using System.Collections.Generic;


namespace ChorebotMessageSender
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler Scheduler = new Scheduler();

            while (Console.ReadLine() != "quit")
            {
                Console.WriteLine("type quit to quit");
                Console.ReadLine();
            }
        }

        
    }
}
