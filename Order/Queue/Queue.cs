using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

public class Queue
{
    public static IModel Connect()
    {
        var factory = new ConnectionFactory()
        {
           HostName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_HOST") ?? "localhost",
           Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PORT") ?? "5672"),
           VirtualHost = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_VHOST") ?? "/",
           UserName = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER") ?? "rabbitmq",
           Password = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS") ?? "rabbitmq"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        return channel;
    }


    public static void Notify(byte[] payload, string exchange, string routingKey, IModel channel)
    {
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: properties,
            body: payload
        );

        Console.WriteLine("Message sent");
    }

    public static void StartConsuming(string queue, IModel ch, Channel<byte[]> inChannel)
    {
        var q = ch.QueueDeclare(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(ch);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            inChannel.Writer.TryWrite(body);
        };

        ch.BasicConsume(
            queue: q.QueueName,
            autoAck: true,
            consumer: consumer);

        inChannel.Reader.Completion.ContinueWith(task =>
        {
            ch.Dispose();
        });
    }
}
