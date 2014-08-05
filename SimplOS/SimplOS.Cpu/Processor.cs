using System;

namespace SimplOS.Cpu
{
    public class Processor
    {
        private bool _checkFlag;
        private String _instructionRegister = String.Empty;
        private String _programCounter = String.Empty;
        private String _register = String.Empty;

        public String GetRegister()
        {
            return _register;
        }

        public void SetRegister(string data)
        {
            _register = data;
        }

        public void IncrementProgramCounter()
        {
            int temp;
            Int32.TryParse(_programCounter, out temp);
            _programCounter = (++temp).ToString();
        }

        public void DecodeExecuteInstruction()
        {
            //string instruction = 
        }
    }
}