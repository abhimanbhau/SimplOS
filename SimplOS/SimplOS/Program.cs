using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SimplOS.Cpu;
using SimplOS.Log;

namespace SimplOS
{
    internal class Program
    {
        private static void Main(String[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var memoryIndex = 0;
            var currentProgramCardOwner = 9999;
            var programCardNumber = 0;
            var jobId = 999;

            if (args.Length < 1 && !File.Exists("input.txt"))
            {
                Console.WriteLine("Usage :\nSimplOS filename.txt\n");
                return;
            }
            var programCard = File.Exists("input.txt") ? "input.txt" : args[0];
            if (!Directory.Exists(@"Log\" + DateTime.UtcNow.ToShortDateString() + "-"))
            {
                Directory.CreateDirectory(@"Log\" + DateTime.Now.ToShortDateString() + "-" + DateTime.UtcNow.Minute + "-" + DateTime.UtcNow.Second);
            }
            var logger = new Logger("Log/" + DateTime.UtcNow.ToShortDateString() + "-" + DateTime.UtcNow.Minute + "-" + DateTime.UtcNow.Second + @"\" + "SimplOS.main.txt");
            var cpu = new Processor();

            var buffer = File.ReadAllLines(programCard);
            logger.LogD("Start");
            logger.LogD("Reading Program Card From -> " + programCard);

            var inputStream = new StreamReader(programCard);
            try
            {
                string currentLine;
                while ((currentLine = inputStream.ReadLine()) != null)
                {
                    if (currentLine.StartsWith("$AMJ"))
                    {
                        programCardNumber++;
                        currentProgramCardOwner = Convert.ToInt16(currentLine.Substring(4, 2));
                        jobId = Convert.ToInt16(currentLine.Substring(6, 2));
                        logger.LogD("Processing Program Card - " + programCardNumber + " -Control -> " + "Roll Number: " +
                            currentProgramCardOwner + ", JobID: " + jobId + " ,No.ofInstructs: "
                            + currentLine.Substring(8, 2) + " ,Length of Data: " + currentLine.Substring(10, 2));
                        memoryIndex = 0;
                    }
                    else if (currentLine.StartsWith("$DTA"))
                    {
                        logger.LogD("Processing Program Card - " + programCardNumber + " -Data");
                        cpu.StartExecution(programCardNumber, ref  inputStream, currentProgramCardOwner, jobId);
                    }
                    else if (currentLine.StartsWith("$END"))
                    {
                        logger.LogD("Processing Program Card - " + programCardNumber + " -End");
                    }
                    else
                    {
                        cpu.Ram.Gd(memoryIndex, currentLine);
                        memoryIndex += 10;
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogE("FATAL EXCEPTION - > " + e.Message + "\n" + e.StackTrace);
            }
            sw.Stop();
            logger.LogD("End");
            logger.LogD(programCardNumber + " Program cards processed in " + sw.ElapsedMilliseconds + " MilliSeconds");
            logger.LogD((double)sw.ElapsedMilliseconds / programCardNumber + " MilliSeconds/Card");
            logger.Finish();
        }
    }
}