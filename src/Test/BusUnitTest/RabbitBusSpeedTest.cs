using Cactus.Bus.RabbitBus;
using Cactus.Protocol.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BusUnitTest
{
    public class RabbitBusSpeedTest
    {
        private RabbitBus _bus = new RabbitBus(new Uri("amqp://activator:activator@192.168.31.102:5672"), "Cactus");

        [Fact]
        public void SpeedTest()
        {
            _bus.Subscribe("Event", _processor);

            Packet packet = new Packet()
            {
                Service = "Output",
                Command = "Print",
                Data = "OK",
            };
            packet.Args.Add(new string[] { "arg1", "arg2" });
            packet.Options.Add(new Dictionary<string, string>() { { "opt1", "1" }, { "opt2", "2" } });

            for (int i = 0; i < 9; i++)
            {
                Task.Factory.StartNew(() => {
                    var bus = new RabbitBus(new Uri("amqp://activator:activator@192.168.31.102:5672"), "Cactus");
                    while (true)
                    {
                        bus.Publish("Event", packet);

                        Thread.Sleep(0);
                    }
                });
            }

            while (true)
            {
                _bus.Publish("Event", packet);

                Thread.Sleep(0);
            }

        }

        private bool _processor(BusChannel channel, Packet packet)
        {
            return true;
        }

    }
}
