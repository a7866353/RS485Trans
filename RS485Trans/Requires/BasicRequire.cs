using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RS485Trans.Requires
{
    class DataWriter
    {
        private List<byte[]> _dataList;
        public DataWriter()
        {
            _dataList = new List<byte[]>();
        }
        public byte[] GetBuffer()
        {
            int len = 0;
            foreach (byte[] dataArr in _dataList)
                len += dataArr.Length;

            byte[] resultBuffer = new byte[len];
            int pos = 0;
            foreach(byte[] dataArr in _dataList)
            {
                dataArr.CopyTo(resultBuffer, pos);
                pos += dataArr.Length;
            }

            return resultBuffer;
        }
        public void Add(byte data)
        {
            _dataList.Add(new byte[]{ data });
        }
        public void Add(FunctionCode data)
        {
            _dataList.Add(new byte[] { (byte)data });

        }
        public void Add(short data)
        {
            _dataList.Add(BitConverter.GetBytes(data));

        }
        public void Add(string data)
        {
            if (data == null)
                data = "";
            _dataList.Add(ASCIIEncoding.ASCII.GetBytes(data));

        }
    }
    class DataReader
    {
        private byte[] _data;
        private int _pos;
        public DataReader(byte[] data)
        {
            _data = data;
            _pos = 0;
        }

        public FunctionCode GetFuncCode()
        {
            byte res = _data[_pos];
            _pos += sizeof(byte);
            return (FunctionCode)res;
        }
        public byte GetByte()
        {
            byte res = _data[_pos];
            _pos += sizeof(byte);
            return res;
        }

        public short GetShort()
        {
            short res = BitConverter.ToInt16(_data, _pos);
            _pos += sizeof(short);
            return res;
        }
        public string GetString()
        {
            string str = ASCIIEncoding.ASCII.GetString(_data, _pos, _data.Length - _pos);
            _pos += str.Length + 1;
            return str;
        }


    }


    enum FunctionCode
    {
        None = 0,
        Result,
        RegiestWriteShort,
        RegiestReadShort,
        LCDWrite,
        LCDRead,
    }


    abstract class BasicRequire
    {

        private Semaphore _workSem;
        private int _timeout = 0;
        private byte[] _resultBuffer;

        protected RequireController.ReqType _ReqType = RequireController.ReqType.Normal;
        abstract protected ErrorCode CovertResult(byte[] data);

        public enum ErrorCode
        {
            OK,
            Error,
            Timeout,
        }

        public BasicRequire()
        {
            _workSem = new Semaphore(0, 1);
        }
        abstract public byte[] GetData();
        public ErrorCode DoRequire()
        {
            RequireController.Controller.AddReq(this, _ReqType);

            if (_workSem.WaitOne(_timeout) == false)
                return ErrorCode.Timeout;
            if (_resultBuffer == null)
                return ErrorCode.Error;

            return CovertResult(_resultBuffer);
        }
        public void SetResult(byte[] data)
        {
            _resultBuffer = data;
            _workSem.Release();
        }


    }
}
