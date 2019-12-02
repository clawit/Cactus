using Cactus.Bus.RabbitBus;
using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BusUnitTest
{
    public class RabbitBusTest
    {
        private RabbitBus _bus = new RabbitBus(new Uri("rabbit://"), "Cactus");

        [Fact]
        public void PublishTest()
        {
            Packet packet = new Packet();
            for (int i = 0; i < 10; i++)
            {
                _bus.Publish("Data", packet);
            }
        }

        [Fact]
        public void SubscribeTest()
        {
            _bus.Subscribe("Data", _processor);
        }

        private bool _processor(BusChannel channel, Packet packet)
        {
            return true;
        }


    }
}
