using NServiceBus;
using Sales.Messages.Commands;
using Sales.Messages.Events;

namespace Sales.Handlers;

public class SubmitOrderHandler : IHandleMessages<SubmitOrder>
{
    public async Task Handle(SubmitOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Sales.SubmitOrderHandler] Received order {message.Identifier}");
        
        await context.Publish(new OrderAccepted() { Identifier = message.Identifier });
    }
}