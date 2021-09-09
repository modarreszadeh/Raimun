using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using Consumer.Messaging;
using Consumer.Messaging.Recive;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = WeatherReceiver.Recevie();
            var weather = JsonConvert.DeserializeObject<WeatherMessage>(message);
            if (weather.Temperature > 14)
            {
                var client = new HttpClient();
                var response = client.PostAsync("https://localhost:5001/api/weather",
                    new StringContent(message, Encoding.UTF8, "application/json"));
                if (response.Result.IsSuccessStatusCode)
                    Console.WriteLine("Successfully added to Database!");
            }
        }
    }
}