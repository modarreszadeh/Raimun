using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.Messaging.Recive
{
    public class WeatherReceiver
    {
        public void Recevie()
        {
            string message = "";
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Password = "guest",
                UserName = "guest"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "Weather",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                message = Encoding.UTF8.GetString(body);

                AddWeather(message);
            };
            channel.BasicConsume(queue: "Weather",
                autoAck: true,
                consumer: consumer);
        }

        private void AddWeather(string message)
        {
            var weather = JsonConvert.DeserializeObject<WeatherMessage>(message);
            Console.WriteLine(weather.CityName);
            if (weather.Temperature > 14)
            {
                var client = new HttpClient();
                var response = client.PostAsync("https://localhost:44374/api/weather",
                    new StringContent(message, Encoding.UTF8, "application/json"));
                if (response.Result.IsSuccessStatusCode)
                    Console.WriteLine("Successfully added to Database!");
            }
        }
    }
}