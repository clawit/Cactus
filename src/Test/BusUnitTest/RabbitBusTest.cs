using Cactus.Bus.RabbitBus;
using Cactus.Protocol.Model;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace BusUnitTest
{
    public class RabbitBusTest
    {
        private RabbitBus _bus = new RabbitBus(new Uri("amqp://activator:activator@192.168.31.102:5672"), "Cactus");

        [Fact]
        public void PublishTest()
        {
            Packet packet = new Packet() {
                Service = "Output",
                Command = "Print",
                Data = "",
            };
            packet.Args.Add(new string[] { "arg1", "arg2" });
            packet.Options.Add(new Dictionary<string, string>() { { "opt1", "1" }, { "opt2", "2" } });

            for (int i = 0; i < 10; i++)
            {
                _bus.Publish("Event", packet);
            }
        }

        [Fact]
        public void SubscribeTest()
        {
            _bus.Subscribe("Event", _processor);
            while (true)
            {
                Thread.Sleep(0);
            }
        }

        private bool _processor(BusChannel channel, Packet packet)
        {
            return true;
        }


    }
}
