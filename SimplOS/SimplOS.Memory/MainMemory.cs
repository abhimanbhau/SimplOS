using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplOS.Memory
{
    public class MainMemory
    {
        private readonly String[,] _memory = new String[100, 1];

        /// <summary>
        ///     Loads data into memory
        /// </summary>
        /// <param name="index">memory location (if not multiple of 10, will be set to just previous multiple of 10)</param>
        /// <param name="data">Actual data, (length max=40, however less than 40 is ok)</param>
        public void GetData(int index, String data)
        {
            index = index - (index % 10);
            data = CorrectString(data);
            IEnumerable<String> instructions = Split(data, 4);
            foreach (String word in instructions)
            {
                _memory[index, 0] = word;
                index++;
            }
        }

        public String[,] PutData(int index)
        {
            index = index - (index % 10);
            var data = new string[10,1];
            Array.Copy(_memory, index, data, 0, 10);
            return data;
        }

        private String CorrectString(String input)
        {
            if (input.Length != 40)
            {
                int length = input.Length;
                for (int i = 0; i < (40 - length); i++)
                {
                    input += "`";
                }
            }
            return input;
        }

        /// <summary>
        ///     Simple string splitter which splits string in equal chunk size
        /// </summary>
        /// <param name="input">String to split</param>
        /// <param name="chunkSize">length of each part</param>
        /// <returns></returns>
        private static IEnumerable<string> Split(string input, int chunkSize)
        {
            return Enumerable.Range(0, input.Length / chunkSize)
                .Select(i => input.Substring(i * chunkSize, chunkSize));
        }

        public string RetrieveInstruction(int location)
        {
            return _memory[location,0];
        }
    }
}