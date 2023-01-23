using NServiceBus;
using Sales.Messages.Commands;
using Sales.Messages.Events;

namespace Sales.Handlers;

public class SubmitOrderHandler : IHandleMessages<PlaceOrder>
{
    public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Sales.SubmitOrderHandler] Received order {message.Identifier}");
        
        await context.Publish(new OrderAccepted() { Identifier = message.Identifier });
    }
}