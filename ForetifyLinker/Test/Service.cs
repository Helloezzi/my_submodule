using Grpc.Core;
using Morai.Protobuf.Foretify;
using System.Threading.Tasks;

namespace ForetifyLinker
{
    class Service : Foretify.ForetifyBase
    {
        // example
        public override Task<init_resp> init(init_req request, ServerCallContext context)
        {
            init_resp resp = new init_resp
            {
                Status = new status
                {
                    Info = { "aa", "bb" },
                    Warning = { "cc" },
                    Error = { "dd" },
                }
            };
            return Task.FromResult(resp);
        }
    }
}
