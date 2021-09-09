using System;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Web.Infrastructure.Model;
using Web.Models.Dtos;

namespace Web.Messaging.Sender
{
    public class WeatherSender : IWeatherSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _queueName;
        private readonly string _username;
        private IConnection _connection;

        public WeatherSender(IOptions<RabbitMqSetting> options)
        {
            _queueName = options.Value.QueueName;
            _hostname = options.Value.Hostname;
            _username = options.Value.UserName;
            _password = options.Value.Password;

            CreateConnection();
        }

        public void Send(WeatherMessage weatherMessage)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false,
                        arguments: null);

                    var json = JsonConvert.SerializeObject(weatherMessage);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                }
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }

    public interface IWeatherSender
    {
        void Send(WeatherMessage weatherMessage);
    }
}