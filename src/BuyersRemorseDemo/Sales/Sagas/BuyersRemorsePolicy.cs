using NServiceBus;
using Sales.Messages.Commands;
using Sales.Messages.Events;

namespace Sales.Sagas;

public class BuyersRemorsePolicy : Saga<BuyersRemorseState>,
    IAmStartedByMessages<SubmitOrder>,
    IHandleTimeouts<BuyersRemorseIsOver>,
    IHandleMessages<CancelOrder>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BuyersRemorseState> mapper)
    {
        mapper.MapSaga(s => s.OrderId)
            .ToMessage<SubmitOrder>(m => m.Identifier)
            .ToMessage<CancelOrder>(m => m.Identifier);
    }

    public async Task Handle(SubmitOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Sales.BuyersRemorsePolicy] Received order {message.Identifier}");
        
        await RequestTimeout(context, TimeSpan.FromSeconds(10), new BuyersRemorseIsOver());
    }

    public async Task Timeout(BuyersRemorseIsOver state, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Sales.BuyersRemorsePolicy] BuyersRemorseIsOver for {Data.OrderId}");

        await context.Publish(new OrderAccepted() { Identifier = Data.OrderId });
        
        MarkAsComplete();
    }

    public Task Handle(CancelOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine($"[Sales.BuyersRemorsePolicy] Received cancellation for order {message.Identifier}");
        
        MarkAsComplete();

        return Task.CompletedTask;
    }
}

public class BuyersRemorseState : ContainSagaData
{
    public Guid OrderId { get; set; }
}