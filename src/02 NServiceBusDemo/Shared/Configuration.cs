using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NServiceBus;

namespace Shared
{
    public static class Configuration
    {
        public static EndpointConfiguration GetConfiguration(string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            //var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(@"data source=localhost,1434; user id=sa; password=mNhzNtO64GdW; Initial Catalog=servicecontrol");

            transport.Routing().RouteToEndpoint(typeof(MyMessage).Assembly, "receiver");

            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();

            return endpointConfiguration;
        }
    }
}
