using StackExchange.Redis;

namespace Order.Db
{
    public static class Db
    {
        public static ConnectionMultiplexer Connect()
        {
            string redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
            Console.WriteLine(redisHost);
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints = { redisHost },
                Password = "",
                DefaultDatabase = 0
            };

            return ConnectionMultiplexer.Connect(options);
        }
    }
}

