using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans.Requires
{
    class LoopbackRequire : BasicRequire
    {
        public byte[] Data { set; get; }
        public byte[] ResultData { set; get; }
        protected override BasicRequire.ErrorCode CovertResult(byte[] data)
        {
            DataReader dr = new DataReader(data);
            FunctionCode funcCode = dr.GetFuncCode();
            if (funcCode != FunctionCode.LoopBack)
                return ErrorCode.Error;
            byte[] resData = new byte[data.Length-1];
            data.CopyTo(resData, 1);
            ResultData = resData;

            return ErrorCode.OK;
        }

        public override byte[] GetData()
        {
            DataWriter dc = new DataWriter();
            dc.Add(FunctionCode.LoopBack);
            dc.Add(Data);

            return dc.GetBuffer();
        }
    }
}
