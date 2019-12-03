using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cactus.Protocol.Interface
{
    public delegate Task<bool> PacketProcessor(BusChannel channel, Packet packet);
    public interface IBus
    {
        bool Publish(BusChannel channel, Packet packet);

        bool Subscribe(BusChannel channel, PacketProcessor processor);
    }
}
