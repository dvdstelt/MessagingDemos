using Billing.Messages.Events;
using NServiceBus;
using Sales.Messages.Events;

namespace Billing.Handlers;

public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
{
    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Billing.OrderAcceptedHandler] Received order {message.Identifier}");
        
        await context.Publish(new OrderBilled() { Identifier = message.Identifier });
    }
}