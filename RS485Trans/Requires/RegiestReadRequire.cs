﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans.Requires
{
    class RegiestReadRequire : BasicRequire
    {
        public byte RegiestAddress { set; get; }
        public override byte[] GetData()
        {
            DataWriter dc = new DataWriter();
            dc.Add((byte)FunctionCode.RegiestWriteShort);
            dc.Add(RegiestAddress);

            return dc.GetBuffer();
        }

        public byte Result { set; get; }
        public short Value { set; get; }

        protected override BasicRequire.ErrorCode CovertResult(byte[] data)
        {
            DataReader dr = new DataReader(data);
            FunctionCode funcCode = (FunctionCode)dr.GetByte();
            if (funcCode == FunctionCode.Result)
            {
                Result = dr.GetByte();
                Value = 0;
            }
            else if (funcCode == FunctionCode.RegiestReadShort)
            {
                Result = 0;
                Value = dr.GetShort();
            }
            else
            {
                return ErrorCode.Error;
            }

            return ErrorCode.OK;
        }
    }
}