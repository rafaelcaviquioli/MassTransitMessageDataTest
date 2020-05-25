using System;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.MessageData;
using Microsoft.Extensions.Configuration;

namespace MassTransitMessageDataTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            
            var messageDataRepository = new InMemoryMessageDataRepository();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var hostUri = new Uri(appSettings["rabbitmq:host"]);
                var username = appSettings["rabbitmq:username"];
                var password = appSettings["rabbitmq:password"];

                cfg.Host(hostUri, host =>
                {
                    host.Username(username);
                    host.Password(password);
                });

                cfg.UseMessageData(
                    messageDataRepository
                );

                cfg.ReceiveEndpoint("my-event-channel", endpoint =>
                {
                    endpoint.Handler<MyEventMessage>(async context =>
                    {
                        Console.Out.Write($"Received the event {context.Message.Name} - ");
                        
                        var file = context.Message.MyFile.Content;
                        if (file.HasValue)
                        {
                            Console.Out.WriteLine("Event file value: " + Encoding.ASCII.GetString(await file.Value));
                        }
                        else
                        {
                            Console.Out.WriteLine("Doesn't have file value");
                        }
                    });
                    
                    endpoint.Handler<ArrayFilesMessage>(async context =>
                    {
                        Console.Out.Write($"\nReceived the event {context.Message.Name} - ");
                        
                        var files = context.Message.Files;
                        if (files.Length > 0)
                        {
                            Console.Out.WriteLine("Array files: " + files.Length.ToString());
                            foreach (var file in files)
                            {
                                if (file.HasValue)
                                {
                                    Console.Out.WriteLine("File: " + Encoding.ASCII.GetString(await file.Value));    
                                }
                                else
                                {
                                    Console.Out.WriteLine("File doesn't exist");
                                }
                            }
                        }
                        else
                        {
                            Console.Out.WriteLine("Doesn't have array files");
                        }
                    });
                });
            });

            try
            {
                await busControl.StartAsync();
                
                Console.WriteLine("Listening for events, press a key to exit...");
                
                var uri = new Uri($"{appSettings["rabbitmq:host"]}my-event-channel");
                var endpoint = await busControl.GetSendEndpoint(uri);

                await endpoint.Send<MyEventMessage>(new
                {
                    Name = "MyEvent with message data",
                    MyFile = new
                    {
                        Name = "My file name",
                        Content = Encoding.ASCII.GetBytes("file content...")
                    },
                });
                
                await endpoint.Send<ArrayFilesMessage>(new
                {
                    Name = "MyEvent with message data",
                    Files = new []
                    {
                        Encoding.ASCII.GetBytes("File 1 content..."),
                        Encoding.ASCII.GetBytes("File 2 content...")
                    },
                });
                
                Console.ReadLine();
            }
            finally
            {
                await busControl.StopAsync();

                Console.WriteLine("Stopping...");
            }
        }
    }
}
