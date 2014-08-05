using System;
using System.Collections;
using System.IO;
using SimplOS.Cpu;
using SimplOS.Memory;
using System.Collections.Generic;

namespace SimplOS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string programCard;
            if (args.Length < 2 && !File.Exists("input.txt"))
            {
                Console.WriteLine("Usage :\nSimplOS filename.txt");
                return;
            }
            else
            {
                if (File.Exists("input.txt"))
                {
                    programCard = "input.txt";
                }
                else
                {
                    programCard = args[1];
                }
            }
            var cpu = new Processor();
            var ram = new MainMemory();

        }
    }
}