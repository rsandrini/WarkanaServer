using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarkanaServer.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class MyCustomOperation : Operation
    {
        public MyCustomOperation(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = 100, IsOptional = false)]
        public string Message { get; set; }

        // GetOperationResponse could be implemented by this class, too
    }
}
