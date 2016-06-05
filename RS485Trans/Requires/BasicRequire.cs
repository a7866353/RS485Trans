using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Trans.Requires
{
    abstract class BasicRequire
    {
        abstract public byte[] GetData();

        abstract public void SetResult(byte[] data);

        protected RequireController.ReqType _ReqType = RequireController.ReqType.Normal;

        public void AddReq()
        {
            RequireController.Controller.AddReq(this, _ReqType);
        }

    }
}
