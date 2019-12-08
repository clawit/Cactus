using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Bus.RabbitBus
{
    public static class PacketExtension
    {
        //public static bool SetDeliveryTag(this Packet packet, UInt64 deliveryTag)
        //{
        //    return packet.SetOption("DeliveryTag", deliveryTag.ToString());
        //}

        //public static bool TryGetDeliveryTag(this Packet packet, out UInt64 deliveryTag)
        //{
        //    if (packet.TryGetOption("DeliveryTag", out string value))
        //    {
        //        deliveryTag = Convert.ToUInt64(value);
        //        return true;
        //    }
        //    else
        //    {
        //        deliveryTag = 0;
        //        return false;
        //    }
        //}

        public static bool SetTriggerAt(this Packet packet, DateTime triggerAt)
        {
            return packet.SetOption("TriggerAt", triggerAt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        public static bool TryGetTriggerAt(this Packet packet, out DateTime triggerAt)
        {
            if (packet.TryGetOption("TriggerAt", out string value))
            {
                return DateTime.TryParse(value, out triggerAt);
            }
            else
            {
                triggerAt = DateTime.MinValue;
                return false;
            }
        }
    }
}
