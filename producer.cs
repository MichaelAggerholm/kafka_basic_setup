using Confluent.Kafka;
using System;
using Microsoft.Extensions.Configuration;

class Producer {
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Please provide the configuration file path as a command line argument");
        }

        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(args[0])
            .Build();

        const string topic = "purchases";

        string[] users = { "eabard", "jsmithd", "sgarciad", "jbernardd", "htanakad", "awaltherd" };
        string[] items = { "bookd", "alarm clockd", "t-shirtsd", "gift cardd", "batteriesd" };

        using (var producer = new ProducerBuilder<string, string>(
            configuration.AsEnumerable()).Build())
        {
            var numProduced = 0;
            const int numMessages = 10;
            for (int i = 0; i < numMessages; ++i)
            {
                Random rnd = new Random();
                var user = users[rnd.Next(users.Length)];
                var item = items[rnd.Next(items.Length)];

                producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
                    (deliveryReport) =>
                    {
                        if (deliveryReport.Error.Code != ErrorCode.NoError) {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else {
                            Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                            numProduced += 1;
                        }
                    });
            }

            producer.Flush(TimeSpan.FromSeconds(10));
            Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
        }
    }
}
