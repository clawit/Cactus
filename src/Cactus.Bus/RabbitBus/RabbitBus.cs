using Cactus.Protocol.Interface;
using Cactus.Protocol.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cactus.Bus.RabbitBus
{
    public class RabbitBus : IBus, IDisposable
    {
        private ConnectionFactory _factory = new ConnectionFactory();
        private IConnection _conncetion = null;
        private IModel _channel = null;
        private IBasicProperties _properties = null;
        private IBasicConsumer _consumer = null;
        private string _exchangeName = null;
        private bool _durable;

        private ConcurrentQueue<Packet>
        private Task _dispatcherTask = null;
        private ConcurrentDictionary<BusChannel, List<PacketProcessor>> _subscribers = new ConcurrentDictionary<BusChannel, List<PacketProcessor>>();

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
            _channel.BasicQos(0, 1, false);
            //设置类型 Topic
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic);

            //
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

            _consumer = new EventingBasicConsumer(_channel);

            var rabbitConncetion = new RabbitConncetion();
            rabbitConncetion.Channel = _channel;
            rabbitConncetion.Subscribers = _subscribers;
            rabbitConncetion.ExchangeName = _exchangeName;
            rabbitConncetion.Consumer = _consumer;
            _dispatcherTask = Task.Factory.StartNew(_dispatcher, rabbitConncetion);
        }

        public bool Publish(BusChannel channel, Packet packet)
        {
            try
            {
                var body = packet.GetByteArray();
                _channel.BasicPublish(_exchangeName, channel, false, _properties, body);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Subscribe(BusChannel channel, PacketProcessor processor)
        {
            if (BindQueue(channel))
            {
                string channelName = GetChannelName(_exchangeName, channel);

                _subscribers.TryAdd(channelName, new List<PacketProcessor>());
                var subscriber = _subscribers[channelName];
                if (!subscriber.Contains(processor))
                {
                    subscriber.Add(processor);
                }

                return true;
            }
            else
                return false;
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
            //新建Queue
            _channel.QueueDeclare(queueName, _durable, false, false, null);
            //绑定 queue 与exchange
            string channelName = GetChannelName(_exchangeName, queueName);
            _channel.QueueBind(queueName, _exchangeName, channelName, null);
            //绑定consumer
            _channel.BasicConsume(channelName, false, _consumer);

            return true;
        }

        private static string GetChannelName(string exchangeName, string queueName)
        { 
            return $"{exchangeName}.{queueName}";
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
                        var channelName = GetChannelName(rabbitConncetion.ExchangeName, channel);
                        rabbitConncetion.Channel.BasicConsume(channelName, false, rabbitConncetion.Consumer);
                    }
                    
                    await aaa();
                }
                catch (Exception ex)
                {
                    //TODO:处理失败的消息

                }
            }
        }

        public static async Task<bool> aaa()
        {
            return false;
        }
    }
}
