using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans.Requires
{
    class LCDReadRequire : BasicRequire
    {
        private int _textLength = 8;
        public byte DisplayLine { set; get; }

        public override byte[] GetData()
        {
            DataWriter dc = new DataWriter();
            dc.Add((byte)FunctionCode.LCDRead);
            dc.Add(DisplayLine);

            return dc.GetBuffer();
        }

        public byte Result { set; get; }
        public string Text { set; get; }
        protected override BasicRequire.ErrorCode CovertResult(byte[] data)
        {
            DataReader dr = new DataReader(data);
            FunctionCode funcCode = (FunctionCode)dr.GetByte();
            if (funcCode == FunctionCode.Result)
            {
                Result = dr.GetByte();
                Text = null;
            }
            else if (funcCode == FunctionCode.LCDRead)
            {
                Result = 0;
                Text = dr.GetString();
            }
            else
            {
                return ErrorCode.Error;
            }

            return ErrorCode.OK;
        }
        public string Read()
        {
            DoRequire();
            return Text;
        }
    }
}
