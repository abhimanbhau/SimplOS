using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SimplOS.Cpu;
using SimplOS.Memory;

namespace SimplOS
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || !File.Exists("input.txt"))
            {
                Console.WriteLine("Usage :\nSimplOS filename.txt");
                return;
            }
            Processor Cpu = new Processor();
            MainMemory Ram = new MainMemory();
        }
    }
}
