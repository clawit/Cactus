using Cactus.Protocol.Interface;
using Cactus.Protocol.Model;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cactus.Service.Grpc
{
    public class BusGrpcService : BusService.BusServiceBase
    {
        private readonly ILogger<BusGrpcService> _logger;
        public BusGrpcService(ILogger<BusGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<RequestResult> Subscribe(Packet packet, ServerCallContext context)
        {
            return Task.FromResult(new RequestResult
            {
                IsOk = true
            });
        }

        public override Task<RequestResult> Publish(Packet packet, ServerCallContext context)
        {
            return Task.FromResult(new RequestResult
            {
                IsOk = true
            });
        }

    }
}
