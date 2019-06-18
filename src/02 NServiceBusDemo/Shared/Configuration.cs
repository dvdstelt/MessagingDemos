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
            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            transport.Routing().RouteToEndpoint(typeof(MyMessage).Assembly, "receiver");

            return endpointConfiguration;
        }
    }
}
