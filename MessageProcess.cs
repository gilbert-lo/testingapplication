using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WindowsServiceApp
{
    public class MessageProcess
    {
        ConnectionFactory factory = new();

        IConnection? cnn;
        IModel? channel;
        string consumerTag = "";

        public void Start()
        {
            factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
            factory.ClientProvidedName = "Rabbit Receiver1 App";

            cnn = factory.CreateConnection();
            channel = cnn.CreateModel(); ;

            string exchangeName = "DemoExchange";
            string routingKey = "demo-routing-key";
            string queueName = "DemoQueue";

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                var body = args.Body.ToArray();

                string message = Encoding.UTF8.GetString(body);

                //Console.WriteLine($"Message Received: {message}");
                string[] lines = new string[] { message };
                File.AppendAllLines(@"C:\Development\QueueMessage.txt", lines);

                channel.BasicAck(args.DeliveryTag, false);
            };

            consumerTag = channel.BasicConsume(queueName, false, consumer);
        }
        public void Stop() 
        {
            if (channel != null)
            {
                channel.BasicCancel(consumerTag);
                channel.Close();
            }

            if (cnn != null)
                cnn.Close();
        }
    }
}
