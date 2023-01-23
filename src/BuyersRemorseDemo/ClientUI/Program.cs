using NServiceBus;
using Sales.Messages.Commands;
using Shared;

const string endpointName = "ClientUI";

var endpointConfiguration = new EndpointConfiguration(endpointName);
endpointConfiguration.Configure(s =>
{
    s.RouteToEndpoint(typeof(SubmitOrder), "Sales");
    s.RouteToEndpoint(typeof(CancelOrder), "Sales");
});

var endpointInstance = await Endpoint.Start(endpointConfiguration);

await RunLoop(endpointInstance);

await endpointInstance.Stop();

static async Task RunLoop(IMessageSession endpointInstance)
{
    var lastOrder = Guid.Empty;

    while (true)
    {
        Console.WriteLine("Press 'P' to place an order, 'C' to cancel last order, or 'Q' to quit.");
        var key = Console.ReadKey();
        Console.WriteLine();
        
        switch (key.Key)
        {
            case ConsoleKey.P:
                // Instantiate the command
                var command = new SubmitOrder
                {
                    Identifier = Guid.NewGuid()
                };

                // Send the command
                Console.WriteLine($"Sending PlaceOrder command, OrderId = {command.Identifier}");
                await endpointInstance.Send(command);

                lastOrder = command.Identifier; // Store order identifier to cancel if needed.
                break;

            case ConsoleKey.C:
                var cancelCommand = new CancelOrder
                {
                    Identifier = lastOrder
                };
                await endpointInstance.Send(cancelCommand)
                    .ConfigureAwait(false);
                Console.WriteLine($"Sent a correlated message to {cancelCommand.Identifier}");
                break;

            case ConsoleKey.Q:
                return;

            default:
                Console.WriteLine("Unknown input. Please try again.");
                break;
        }        
    }
}