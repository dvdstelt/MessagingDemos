using NServiceBus;
using Sales.Messages.Events;

namespace Shipping.Handlers;

public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
{
    public Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Shipping.OrderAcceptedHandler] Received order {message.Identifier}");
        
        return Task.CompletedTask;
    }
}