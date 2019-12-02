using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Protocol.Interface
{
    public delegate bool PacketProcessor(BusChannel channel, Packet packet);
    public interface IBus
    {
        bool Publish(BusChannel channel, Packet packet);

        bool Subscribe(BusChannel channel, PacketProcessor processor);
    }
}
