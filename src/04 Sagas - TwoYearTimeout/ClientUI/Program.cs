using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace ClientUI
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "ClientUI";

            var endpointConfiguration = new EndpointConfiguration("ClientUI");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(PlaceOrder), "Sales");
            routing.RouteToEndpoint(typeof(UpdateOrder), "Sales");

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            await RunLoop(endpointInstance)
                .ConfigureAwait(false);

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static ILog log = LogManager.GetLogger<Program>();
        static readonly int maxSagas = 1000;

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            var uniqueGuids = new Guid[maxSagas];
            Parallel.For(0, maxSagas, i => uniqueGuids[i] = Guid.NewGuid());

            while (true)
            {
                log.Info($"\nPress 'G' to generate a {maxSagas} sagas\nPress 'U' to update all {maxSagas}\nPress 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.G:

                        Parallel.For(0, maxSagas, async i =>
                        {
                            // Instantiate the command
                            var command = new PlaceOrder
                            {
                                OrderId = uniqueGuids[i]
                            };

                            // Send the command
                            log.Info($"Sending PlaceOrder command, OrderId = { command.OrderId}");
                            var options = new SendOptions();
                            options.RequireImmediateDispatch();
                            await endpointInstance.Send(command, options).ConfigureAwait(false);
                        });

                        break;

                    case ConsoleKey.U:
                        Parallel.For(0, maxSagas, async i =>
                        {
                            // Instantiate the command
                            var command = new UpdateOrder
                            {
                                OrderId = uniqueGuids[i]
                            };

                            // Send the command
                            log.Info($"Sending UpdateOrder command, OrderId = { command.OrderId}");
                            var options = new SendOptions();
                            options.RequireImmediateDispatch();
                            await endpointInstance.Send(command, options).ConfigureAwait(false);
                        });

                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}