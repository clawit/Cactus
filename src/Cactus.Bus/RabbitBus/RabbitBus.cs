using Cactus.Protocol.Interface;
using Cactus.Protocol.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cactus.Bus.RabbitBus
{
    public class RabbitBus : IBus, IDisposable
    {
        private ConnectionFactory _factory = new ConnectionFactory();
        private IConnection _conncetion = null;
        private IModel _channel = null;
        private IBasicProperties _properties = null;
        private string _exchangeName = null;
        private bool _durable;

        private Task _dispatcherTask = null;
        private ConcurrentDictionary<string, ConcurrentQueue<Packet>> _queues = new ConcurrentDictionary<string, ConcurrentQueue<Packet>>();
        private ConcurrentDictionary<string, List<PacketProcessor>> _subscribers = new ConcurrentDictionary<string, List<PacketProcessor>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conncetionUri"></param>
        /// <param name="busName"></param>
        /// <param name="durable">是否设置持久化</param>
        public RabbitBus(Uri conncetionUri, string busName, bool durable = true)
        {
            _exchangeName = busName;
            _durable = durable;

            _factory.Uri = conncetionUri;
            _conncetion = _factory.CreateConnection();
            _channel = _conncetion.CreateModel();
            //_channel.BasicQos(0, 1, false);
            //设置类型 Topic
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic);

            //
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

            var rabbitConncetion = new RabbitConncetion();
            rabbitConncetion.Channel = _channel;
            rabbitConncetion.Subscribers = _subscribers;
            rabbitConncetion.ExchangeName = _exchangeName;
            rabbitConncetion.Queues = _queues;
            _dispatcherTask = Task.Factory.StartNew(_dispatcher, rabbitConncetion);
        }

        public bool Publish(BusChannel channel, Packet packet)
        {
            try
            {
                if (BindQueue(channel))
                {
                    var body = packet.GetByteArray();
                    var router = $"{GetChannelName(_exchangeName, channel)}.{packet.Service}";
                    _channel.BasicPublish(_exchangeName, router, false, _properties, body);
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool Subscribe(BusChannel channel, PacketProcessor processor)
        {
            if (BindQueue(channel))
            {
                string channelName = GetChannelName(_exchangeName, channel);

                //queue
                _queues.TryAdd(channelName, new ConcurrentQueue<Packet>());

                //processor
                List<PacketProcessor> subscriber = null;
                if (_subscribers.ContainsKey(channelName))
                {
                    subscriber = _subscribers[channelName];
                }
                else
                {
                    subscriber = new List<PacketProcessor>();
                }
                AddProcessor2Subscriber(processor, subscriber);
                _subscribers.TryAdd(channelName, subscriber);

                //consumer
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, args) =>
                {
                    var packet = args.Body.FromByteArray();
                    packet.SetDeliveryTag(args.DeliveryTag);
                    InternalEnqueue(channelName, packet);
                };
                _channel.BasicConsume(channelName, false, consumer);

                return true;
            }
            else
                return false;
        }

        public void InternalEnqueue(string channelName, Packet packet)
        {
            var queue = _queues[channelName];
            queue.Enqueue(packet);
        }

        public void Dispose()
        {
            _channel.Close();
            _conncetion.Close();
            _channel.Dispose();
            _conncetion.Dispose();
        }
        private bool BindQueue(string queueName)
        {
            string channelName = GetChannelName(_exchangeName, queueName);
            //新建Queue
            _channel.QueueDeclare(channelName, _durable, false, false, null);
            //绑定 queue 与exchange
            var router = $"{channelName}.*";
            _channel.QueueBind(channelName, _exchangeName, router, null);

            return true;
        }

        private static string GetChannelName(string exchangeName, string queueName)
        { 
            return $"{exchangeName}.{queueName}";
        }

        private static void AddProcessor2Subscriber(PacketProcessor processor, List<PacketProcessor> subscriber)
        {
            if (!subscriber.Contains(processor))
            {
                subscriber.Add(processor);
            }
        }

        private static async Task _dispatcher(object param)
        {
            var rabbitConncetion = (RabbitConncetion)param;
            while (true)
            {
                try
                {
                    foreach (var channel in rabbitConncetion.Subscribers.Keys)
                    {
                        var queue = rabbitConncetion.Queues[channel];
                        var processors = rabbitConncetion.Subscribers[channel];
                        if (queue!= null && processors != null 
                            && !queue.IsEmpty)
                        {
                            queue.TryDequeue(out var packet);
                            foreach (var processor in processors)
                            {
                                //回调业务处理方法
                                bool result = await processor(channel, packet);

                                packet.TryGetDeliveryTag(out UInt64 deliveryTag);
                                //调用成功则确认
                                if (result)
                                {
                                    rabbitConncetion.Channel.BasicAck(deliveryTag, false);
                                }
                                else
                                {
                                    rabbitConncetion.Channel.BasicReject(deliveryTag, true);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO:处理失败的消息

                }
                Thread.Sleep(0);
            }
        }
    }
}
