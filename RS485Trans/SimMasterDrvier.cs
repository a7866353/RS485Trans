using RS485Trans.Requires;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans
{
    interface IFunction
    {
        byte[] Do(byte[] frame);
    }

    class ResultFunction : IFunction
    {
        private byte _result;
        public ResultFunction(byte result)
        {
            _result = result;
        }
        public byte[] Do(byte[] frame)
        {
            DataWriter dw = new DataWriter();
            dw.Add(FunctionCode.Result);
            dw.Add(_result);

            return dw.GetBuffer();
        }
    }
    class RegiestFunction : IFunction
    {
        private short[] _regiestArr;

        public RegiestFunction()
        {
            _regiestArr = new short[10];
        }

        public byte[] Do(byte[] frame)
        {
            DataWriter dw = new DataWriter();
            DataReader dr = new DataReader(frame);
            FunctionCode funcCode = dr.GetFuncCode();

            byte regAddr = dr.GetByte();
            if (regAddr >= _regiestArr.Length)
            {
                ResultFunction rf = new ResultFunction(1);
                return rf.Do(null);
            }

            if( funcCode == FunctionCode.RegiestReadShort )
            {
                dw.Add(funcCode);
                dw.Add(_regiestArr[regAddr]);
                return dw.GetBuffer();
            }
            else if( funcCode == FunctionCode.RegiestWriteShort )
            {
                _regiestArr[regAddr] = dr.GetShort();

                ResultFunction rf = new ResultFunction(0);
                return rf.Do(null);
            }
            else
            {
                ResultFunction rf = new ResultFunction(1);
                return rf.Do(null);
            }
        }

    }
    class LcdFunction : IFunction
    {
        private string[] _lcdDispLineArr;
        private const int _lcdDispLineMax = 4;

        public LcdFunction()
        {
            _lcdDispLineArr = new string[_lcdDispLineMax];
        }

        public byte[] Do(byte[] frame)
        {
            DataWriter dw = new DataWriter();
            DataReader dr = new DataReader(frame);
            FunctionCode funcCode = dr.GetFuncCode();

            byte dispLine = dr.GetByte();
            if (dispLine >= _lcdDispLineArr.Length)
            {
                ResultFunction rf = new ResultFunction(1);
                return rf.Do(null);
            }

            if (funcCode == FunctionCode.LCDRead)
            {
                dw.Add(funcCode);
                dw.Add(_lcdDispLineArr[dispLine]);
                return dw.GetBuffer();
            }
            else if (funcCode == FunctionCode.LCDWrite)
            {
                _lcdDispLineArr[dispLine] = dr.GetString();

                ResultFunction rf = new ResultFunction(0);
                return rf.Do(null);
            }
            else
            {
                ResultFunction rf = new ResultFunction(1);
                return rf.Do(null);
            }
        }

    }
    class LoopBackFunction : IFunction
    {

        public byte[] Do(byte[] frame)
        {
            throw new NotImplementedException();
        }
    }

    class SimMasterDrvier : IMasterDriver
    {
        private IFunction[] _devArr;
        
        public SimMasterDrvier()
        {
            LcdFunction lcd = new LcdFunction();
            RegiestFunction reg = new RegiestFunction();
            
            _devArr = new IFunction[]
            {
                new ResultFunction(1),      // None
                new ResultFunction(1),      // Result
                reg,                        // RegiestWriteShort
                reg,                        // RegiestReadShort
                lcd,                        // LCDWrite
                lcd,                        // LCDRead
            };

        }
        public DataFrame Send(DataFrame frame)
        {
            DataFrame res = new DataFrame();
            res.SalveAddress = frame.SalveAddress;

            DataReader dr = new DataReader(frame.Data);
            int funcCode = dr.GetByte();
            res.Data = _devArr[funcCode].Do(frame.Data);
            res.Length = (byte)res.Data.Length;

            return res;
        }
    }
}
