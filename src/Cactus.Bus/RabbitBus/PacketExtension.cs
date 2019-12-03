using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Bus.RabbitBus
{
    public static class PacketExtension
    {
        public static bool SetDeliveryTag(this Packet packet, UInt64 deliveryTag)
        {
            return packet.SetOption("DeliveryTag", deliveryTag.ToString());
        }

        public static bool TryGetDeliveryTag(this Packet packet, out UInt64 deliveryTag)
        {
            if (packet.TryGetOption("DeliveryTag", out string value))
            {
                deliveryTag = Convert.ToUInt64(value);
                return true;
            }
            else
            {
                deliveryTag = 0;
                return false;
            }
        }
    }
}
