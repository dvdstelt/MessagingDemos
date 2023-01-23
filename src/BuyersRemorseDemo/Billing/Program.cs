using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Shared;

const string endpointName = "Billing";

var host = Host.CreateDefaultBuilder((string[])args)
    .UseNServiceBus(context =>
    {
        var endpoint = new EndpointConfiguration(endpointName);
        endpoint.Configure();

        return endpoint;
    }).Build();

var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();

Console.Title = hostEnvironment.ApplicationName;

host.Run();