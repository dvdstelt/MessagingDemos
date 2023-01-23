using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Receiver";
            var endpointConfiguration = Configuration.GetConfiguration("Receiver");
            endpointConfiguration.Recoverability().Immediate(s => s.NumberOfRetries(0));
            endpointConfiguration.Recoverability().Delayed(s =>
            {
                s.NumberOfRetries(0);
                s.TimeIncrease(TimeSpan.FromSeconds(2));
            });

            var metrics = endpointConfiguration.EnableMetrics();

            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: "Particular.Monitoring",
                interval: TimeSpan.FromSeconds(10));

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}
