using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans
{
    interface IFunction
    {
        byte[] Do(byte[] frame);
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
            throw new NotImplementedException();
        }

    }
    class LcdFunction : IFunction
    {
        private string[] _lcdDispLineArr;

        public LcdFunction()
        {
            _lcdDispLineArr = new string[4];
        }

        public byte[] Do(byte[] frame)
        {
            throw new NotImplementedException();
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
                new LoopBackFunction(),     // 0
                reg,
                reg,
                lcd,
                lcd,
            };

        }
        public DataFrame Send(DataFrame frame)
        {
            DataFrame res = new DataFrame();
            res.SalveAddress = frame.SalveAddress;

            int funcCode = 1;
            res.Data = _devArr[funcCode].Do(frame.Data);
            res.Length = (byte)res.Data.Length;

            return res;
        }
    }
}
