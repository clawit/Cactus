using System;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Protocol.Model
{
    public class BusChannel
    {
        private string ChannelName { get; set; }
        public BusChannel(string channel)
        {
            ChannelName = channel;
        }

        public static implicit operator BusChannel(string value)
        {
            return new BusChannel(value);
        }

        public static implicit operator string(BusChannel value)
        {
            return value.ChannelName;
        }
    }
}
