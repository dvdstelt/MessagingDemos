using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Sales
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "Sales";

            var endpointConfiguration = new EndpointConfiguration("Sales");
            endpointConfiguration.EnableInstallers();

            endpointConfiguration.UseTransport<LearningTransport>();
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(() => new SqlConnection(@"server=.\sqlexpress;database=nservicebus;Trusted_Connection=True"));
            
            endpointConfiguration.SendFailedMessagesTo("error");

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Delayed(delayed =>
            {
                delayed.NumberOfRetries(2);
                delayed.TimeIncrease(TimeSpan.FromSeconds(5));
            });

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}