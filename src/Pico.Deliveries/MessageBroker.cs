using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Pico.Deliveries
{
    public class MessageBroker
    {
        private readonly IConnection _connection;

        public MessageBroker(IConnection connection)
        {
            _connection = connection;
        }

        public void Send(object message, string exchange, string routingKey)
        {
            using (var channel = _connection.CreateModel())
            {
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                var properties = channel.CreateBasicProperties();
                channel.ExchangeDeclare(exchange, ExchangeType.Topic);
                channel.BasicPublish(exchange, routingKey, properties, body);
            }
        }
        
        public class Options
        {
            public string HostName { get; set; }
            public int Port { get; set; }
            public string VirtualHost { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}