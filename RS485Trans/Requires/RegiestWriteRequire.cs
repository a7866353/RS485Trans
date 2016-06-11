using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans.Requires
{
    class RegiestWriteRequire : BasicRequire
    {
        public byte RegiestAddress { set; get; }
        public short Value { set; get; }
        public override byte[] GetData()
        {
            DataWriter dc = new DataWriter();
            dc.Add((byte)FunctionCode.RegiestWriteShort);
            dc.Add(RegiestAddress);
            dc.Add(Value);
            
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

    }
}
