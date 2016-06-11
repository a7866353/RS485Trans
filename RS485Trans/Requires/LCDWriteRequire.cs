using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans.Requires
{
    class LCDWriteRequire : BasicRequire
    {
        private int _textLength = 8;
        public byte DisplayLine { set; get; }
        public string Text { set; get; }

        public override byte[] GetData()
        {
            DataWriter dc = new DataWriter();
            dc.Add((byte)FunctionCode.LCDWrite);
            dc.Add(DisplayLine);

            string str = Text;
            if (Text.Length > _textLength)
            {
                str = Text.Substring(0, _textLength);
            }
            else if(Text.Length < _textLength)
            {
                str = Text.PadRight(_textLength, ' ');
            }
            dc.Add(str);

            return dc.GetBuffer();
        }

        public byte Result { set; get; }
        protected override BasicRequire.ErrorCode CovertResult(byte[] data)
        {
            DataReader dr = new DataReader(data);
            FunctionCode funcCode = (FunctionCode)dr.GetByte();
            if (funcCode != FunctionCode.Result)
                return ErrorCode.Error;

            Result = dr.GetByte();
            return ErrorCode.OK;
        }

        public int Send(string str)
        {
            Text = str;
            DoRequire();
            return Result;
        }
    }
}
