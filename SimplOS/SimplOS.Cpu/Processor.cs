using System;
using System.IO;
using SimplOS.Log;
using SimplOS.Memory;

namespace SimplOS.Cpu
{
    public class Processor
    {
        private readonly Logger _logger =
            new Logger("Log/" + DateTime.UtcNow.ToShortDateString() + "-" + DateTime.UtcNow.Minute + "-" +
                       DateTime.UtcNow.Second + @"\" + "SimplOS.CPU.txt");

        private readonly StreamWriter _output = new StreamWriter("output.txt");
        public MainMemory Ram = new MainMemory();
        private bool _checkFlag;
        private int _currentProgramCardOwner;
        private bool _dtaEnd;
        private String _ic = "00";
        private String _ir = String.Empty;
        private int _jobId;
        private String _register = String.Empty;
        private int _serviceOperand;
        private StreamReader _sr;

        public Processor()
        {
            _logger.LogD("Start");
        }

        public void LoadInstruction()
        {
            _ir = Ram.RetrieveWord(Convert.ToInt16(_ic));
        }

        public void LoadRegister(int location)
        {
            _register = Ram.RetrieveWord(location);
        }

        public void IncrementInstructionCounter()
        {
            try
            {
                int temp;
                Int32.TryParse(_ic, out temp);
                _ic = (++temp).ToString();
                // _logger.LogD("IC++");
            }
            catch (Exception e)
            {
                _logger.LogE("Exception at IC++ -> " + e.Message + "\n\n" + e.StackTrace);
            }
        }

        public void ResetInstructionCounter()
        {
            _logger.LogD("Resetting IC");
            _ic = (0).ToString();
        }

        public void SetInstructionRegister(int address)
        {
            _logger.LogD("Setting IC to " + address);
            _ir = (address).ToString();
        }

        public void StartExecution(int position, ref StreamReader sr, int programCardNumber, int jobId)
        {
            _currentProgramCardOwner = programCardNumber;
            _jobId = jobId;
            _sr = sr;
            bool terminate = false;
            while (!terminate)
            {
                string op;
                string operand;
                LoadInstruction();
                if (_ir == null)
                {
                    _logger.LogE("Instruction Null, Error in program card, Halting CPU. ProgramCardOwner -> "
                                 + _currentProgramCardOwner + " JobID -> " + _jobId);
                    break;
                }
                if (_ir.Length == 4)
                {
                    op = _ir.Substring(0, 2);
                    operand = _ir.Substring(2);
                }
                else
                {
                    _logger.LogE("Invalid Instruction -> " + _ir + "ProgramCardOwner -> "
                                 + _currentProgramCardOwner + " JobID -> " + _jobId);
                    continue;
                }
                if (op.Contains("H"))
                {
                    op = op.Substring(0, 1);
                }

                switch (op)
                {
                    case "GD":
                        try
                        {
                            _serviceOperand = Convert.ToInt16(operand);
                        }
                        catch (Exception e)
                        {
                            _logger.LogE("Exception at GD -> ProgramCardOwner -> "
                                         + _currentProgramCardOwner + " JobID -> " + _jobId + "\n" + e.Message + "\n\n" +
                                         e.StackTrace);
                        }
                        _logger.LogD("Service Interrupt Type 1");
                        MasterMode(1);
                        break;

                    case "PD":
                        try
                        {
                            _serviceOperand = Convert.ToInt16(operand);
                        }
                        catch (Exception e)
                        {
                            _logger.LogE("Exception at PD -> ProgramCardOwner -> "
                                         + _currentProgramCardOwner + " JobID -> " + _jobId + "\n" + e.Message + "\n\n" +
                                         e.StackTrace);
                        }
                        _logger.LogD("Service Interrupt Type 2");
                        MasterMode(2);
                        break;

                    case "LR":
                        LoadRegister(Convert.ToInt16(operand));
                        break;

                    case "SR":
                        Ram.StoreWord(Convert.ToInt16(operand), _register);
                        break;

                    case "CR":
                        try
                        {
                            if (Ram.RetrieveWord(Convert.ToInt16(operand)) == _register)
                                _checkFlag = true;
                        }
                        catch (Exception e)
                        {
                            _logger.LogE("Exception at CR -> ProgramCardOwner -> "
                                         + _currentProgramCardOwner + " JobID -> " + _jobId + "\n" + e.Message + "\n\n" +
                                         e.StackTrace);
                        }
                        break;

                    case "BT":
                        try
                        {
                            if (_checkFlag)
                            {
                                SetInstructionRegister(Convert.ToInt16(operand));
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogE("Exception at BT -> ProgramCardOwner -> "
                                         + _currentProgramCardOwner + " JobID -> " + _jobId + "\n" + e.Message + "\n\n" +
                                         e.StackTrace);
                        }
                        break;

                    case "H":
                        MasterMode(3);
                        _logger.LogD("Service Interrupt Type 2");
                        terminate = true;
                        break;
                }

                IncrementInstructionCounter();
            }
            ResetInstructionCounter();
            _output.Flush();
            Ram.Flush();
            _dtaEnd = false;
        }

        private void MasterMode(int interrupt)
        {
            switch (interrupt)
            {
                case 1: // GD
                    string line = string.Empty;
                    if (!_dtaEnd)
                    {
                        line = _sr.ReadLine();
                    }
                    else
                    {
                        _logger.LogE("No More Data Available, ProgramCardOwner -> " + _currentProgramCardOwner +
                                     ", JobID -> " + _jobId);
                        return;
                    }
                    if (line.Contains("$END"))
                    {
                        _dtaEnd = true;
                        return;
                    }
                    Ram.Gd(_serviceOperand, line);
                    break;

                case 2: // PD
                    _output.WriteLine(Ram.Pd(_serviceOperand));
                    break;

                case 3:
                    _output.WriteLine("!--Program Card End--![Roll-" + _currentProgramCardOwner + " ,JobId-" + _jobId + "]");
                    _logger.LogD("CPU HALT");
                    break;
            }
        }
    }
}