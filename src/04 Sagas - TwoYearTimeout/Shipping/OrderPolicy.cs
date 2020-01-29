using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Shipping
{
    using System;

    public class OrderPolicy : Saga<OrderPolicyState>,
        IAmStartedByMessages<OrderPlaced>,
        IAmStartedByMessages<OrderBilled>,
        IHandleTimeouts<OrderExpiredTimeout>
    {
        static ILog log = LogManager.GetLogger<OrderPolicy>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderPolicyState> mapper)
        {
            mapper.ConfigureMapping<OrderPlaced>(m => m.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<OrderBilled>(m => m.OrderId).ToSaga(s => s.OrderId);
        }

        public async Task Handle(OrderBilled message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderBilled, OrderId = {message.OrderId} - Should we ship now?");
            Data.OrderBilled = true;
            await ContinueProcess(context);
        }


        public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"Received OrderPlaced, OrderId = {message.OrderId} - Should we ship now?");
            Data.OrderPlaced = true;
            await ContinueProcess(context);
        }

        async Task ContinueProcess(IMessageHandlerContext context)
        {
            if (Data.OrderPlaced && Data.OrderBilled)
            {
                log.Info("\tYes we should...");
                var message = new OrderCompleted
                {
                    OrderId = Data.OrderId
                };
                await context.Publish(message);
                MarkAsComplete();
            }
            else
            {
                log.Info($"Requesting timeout, OrderId = {Data.OrderId}.");
                await RequestTimeout<OrderExpiredTimeout>(context, TimeSpan.FromSeconds(5));
            }
            
        }

        public async Task Timeout(OrderExpiredTimeout state, IMessageHandlerContext context)
        {
            log.Info($"Timeout expired, OrderId = {Data.OrderId}.");

            var message = new OrderExpired
            {
                OrderId = Data.OrderId
            };
            await context.Publish(message);
            MarkAsComplete();
        }
    }

    public class OrderPolicyState : ContainSagaData
    {
        public string OrderId { get; set; }
        public bool OrderBilled { get; set; }
        public bool OrderPlaced { get; set; }
    }

    public class OrderExpiredTimeout : IMessage { }
}