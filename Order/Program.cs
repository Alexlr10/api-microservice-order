using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace Order
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var param = args.FirstOrDefault() ?? "";
            Console.WriteLine(param);
            var inChannel = Channel.CreateUnbounded<byte[]>();
            var connection = Queue.Connect();
            switch (param)
            {
                case "checkout":
                    Queue.StartConsuming("checkout_queue", connection, inChannel);

                    await foreach (var payload in inChannel.Reader.ReadAllAsync())
                    {
                        NotifyOrderCreated(CreateOrder(payload), connection);
                        Console.WriteLine(Encoding.UTF8.GetString(payload));
                    }

                    break;
                case "payment":
                    Queue.StartConsuming("payment_queue", connection, inChannel);

                    await foreach (var payload in inChannel.Reader.ReadAllAsync())
                    {
                        var order = JsonConvert.DeserializeObject<OrderModel>(Encoding.UTF8.GetString(payload));
                        SaveOrder(order);
                        Console.WriteLine("Payment: " + Encoding.UTF8.GetString(payload));
                    }

                    break;
                default:
                    Console.WriteLine("Invalid parameter. Usage: checkout | payment");
                    break;
            }
        }



        static OrderModel CreateOrder(byte[] payload)
        {
            var order = JsonConvert.DeserializeObject<OrderModel>(Encoding.UTF8.GetString(payload));

            var uuid = Guid.NewGuid();
            order.Uuid = uuid.ToString();
            order.Status = "pendente";
            order.CreatedAt = DateTime.Now;
            SaveOrder(order);
            return order;
        }

        static void SaveOrder(OrderModel order)
        {
            var json = JsonConvert.SerializeObject(order);
            var connection = Db.Db.Connect();
            var result = connection.GetDatabase().StringSet(order.Uuid, json, null);

            if (!result)
            {
                throw new Exception("Failed to save order.");
            }
        }

        static void NotifyOrderCreated(OrderModel order, IModel channel)
        {
            var json = JsonConvert.SerializeObject(order);
            var body = Encoding.UTF8.GetBytes(json);
            Queue.Notify(body, "order_ex", "", channel);
        }
    }

}