﻿using Cactus.Protocol.Interface;
using Cactus.Protocol.Model;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Bus.RabbitBus
{
    public class RabbitConncetion
    {
        public IModel Channel { get; set; }

        public ConcurrentDictionary<string, List<PacketProcessor>> Subscribers { get; set; }
    
        public ConcurrentDictionary<string, ConcurrentQueue<Packet>> Queues { get; set; }
        public string ExchangeName { get; set; }

    }
}
