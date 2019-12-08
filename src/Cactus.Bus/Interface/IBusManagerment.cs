using Cactus.Protocol.Interface;
using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cactus.Bus.Interface
{
    public interface IBusManagerment : IBus
    {
        Task InternalInvoke(string channel, Packet packet);
    }
}
