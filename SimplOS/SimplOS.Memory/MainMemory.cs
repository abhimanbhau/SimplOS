using System;
using System.Collections.Generic;
using System.Linq;
using SimplOS.Log;

namespace SimplOS.Memory
{
    public class MainMemory
    {
        private const string AppendChar = " ";
        private readonly String[,] _memory = new String[100, 1];
        readonly Logger _logger = new Logger("Log/" + "SimplOS.Memory.txt");

        public MainMemory()
        {

            _logger.LogD("Start");
        }

        public void Gd(int index, String data)
        {
            if (index > 99)
            {
                _logger.LogE("Index > 100");
                return;
            }
            index = index - (index % 10);
            _logger.LogD("GD at location " + index);
            data = CorrectString(data);
            var instructions = Split(data, 4);
            foreach (var word in instructions)
            {
                StoreWord(index, word);
                index++;
            }
        }

        public void StoreWord(int location, string word)
        {
            if (location > 99)
            {
                _logger.LogE("Index > 100");
                return;
            }
            _logger.LogD("SR at location " + location);
            _memory[location, 0] = word;
        }

        public string RetrieveWord(int location)
        {
            if (location > 99)
            {
                _logger.LogE("Index > 100");
                return null;
            }
            _logger.LogD("LR from location " + location);
            return _memory[location, 0];
        }

        public String Pd(int index)
        {
            if (index > 99)
            {
                _logger.LogE("Index > 100");
                return null;
            }
            index = index - (index % 10);
            _logger.LogD("PD from location " + index);
            var op = String.Empty;
            for (var i = index; i < index + 10; ++i)
            {
                op += _memory[i, 0];
            }
            return op;
        }

        public void Flush()
        {
            _logger.LogD("Flushing Memory and Init it to NULL");
            for (var i = 0; i < 100; ++i)
            {
                _memory[i, 0] = null;
            }
        }

        private String CorrectString(String input)
        {
            _logger.LogD(@"Correcting input string length to 40 by appending " + AppendChar);
            if (input.Length != 40)
            {
                var length = input.Length;
                for (var i = 0; i < (40 - length); i++)
                {
                    input += AppendChar;
                }
            }
            return input;
        }

        private static IEnumerable<string> Split(string input, int chunkSize)
        {
            return Enumerable.Range(0, input.Length / chunkSize)
                .Select(i => input.Substring(i * chunkSize, chunkSize));
        }
    }
}