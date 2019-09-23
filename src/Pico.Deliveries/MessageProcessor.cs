using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Pico.Deliveries
{
    public class MessageProcessor : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly ILogger<MessageProcessor> _logger;
        private readonly IModel _channel;

        public MessageProcessor(IConnection connection, ILogger<MessageProcessor> logger)
        {
            _connection = connection;
            _logger = logger;
            _channel = _connection.CreateModel();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            const string queue = "deliveries-service/orders.order_created";
            _channel.ExchangeDeclare("orders", ExchangeType.Topic);
            _channel.QueueDeclare(queue);
            _channel.QueueBind(queue, "orders", "order_created");
            _channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Received a message: {message}");
                _channel.BasicAck(args.DeliveryTag, false);
            };
            _channel.BasicConsume(queue, false, consumer);

            return Task.CompletedTask;
        }
        
        public override void Dispose()  
        {  
            _channel.Close();  
            _connection.Close();  
            base.Dispose();
        }  
    }
}