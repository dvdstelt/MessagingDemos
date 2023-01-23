namespace Shared;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration Configure(
        this EndpointConfiguration endpointConfiguration,
        Action<RoutingSettings<LearningTransport>> configureRouting = null)
    {
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

        var transport = endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.UsePersistence<LearningPersistence>();
        
        var routing = transport.Routing();

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");

        var conventions = endpointConfiguration.Conventions();

        conventions.DefiningCommandsAs(t =>
            t.Namespace != null && t.Namespace.Contains("Messages") && t.Namespace.EndsWith("Commands"));
        conventions.DefiningEventsAs(t =>
            t.Namespace != null && t.Namespace.Contains("Messages") && t.Namespace.EndsWith("Events"));

        var metrics = endpointConfiguration.EnableMetrics();

        metrics.SendMetricDataToServiceControl(
            serviceControlMetricsAddress: "particular.monitoring",
            interval: TimeSpan.FromSeconds(1));
        
        endpointConfiguration.EnableInstallers();

        configureRouting?.Invoke(routing);

        return endpointConfiguration;
    }
}