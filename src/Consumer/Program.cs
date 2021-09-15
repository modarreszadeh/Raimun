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
            new WeatherReceiver().Recevie();
        }
    }
}