using Billing.Messages.Events;
using NServiceBus;

namespace Shipping.Handlers;

public class OrderBilledHandler : IHandleMessages<OrderBilled>
{
    public Task Handle(OrderBilled message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Shipping.OrderBilledHandler] Received order {message.Identifier}");

        return Task.CompletedTask;
    }
}