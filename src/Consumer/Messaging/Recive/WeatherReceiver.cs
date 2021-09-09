using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Messaging.Recive
{
    public class WeatherReceiver
    {
        public static string Recevie()
        {
            string message = "";
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    message = Encoding.UTF8.GetString(body);
                };
                channel.BasicConsume(queue: "hello",
                    autoAck: true,
                    consumer: consumer);
            }

            return message;
        }
    }
}