using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Sales
{
    public class OrderSaga : Saga<OrderSaga.MySagaData>,
        IAmStartedByMessages<PlaceOrder>,
        IHandleMessages<UpdateOrder>,
        IHandleTimeouts<OrderSaga.RemoveSagaTimeout>
    {
        private static readonly TimeSpan MaxLifeTimeSaga = TimeSpan.FromSeconds(30);

        static ILog log = LogManager.GetLogger<OrderSaga>();

        public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            var timenow = DateTime.UtcNow;
            var timeForNextTimeout = timenow.Add(MaxLifeTimeSaga);

            await RequestTimeout(context, timeForNextTimeout, new RemoveSagaTimeout() { timeSent = timenow });
            Data.LastUpdated = timenow;
        }

        public Task Handle(UpdateOrder message, IMessageHandlerContext context)
        {
            Data.LastUpdated = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public async Task Timeout(RemoveSagaTimeout state, IMessageHandlerContext context)
        {
            //var timeSentHeader = context.MessageHeaders["NServiceBus.TimeSent"];
            //var timeSent = DateTimeExtensions.ToUtcDateTime(timeSentHeader);

            if (state.timeSent >= Data.LastUpdated)
            {
                //log.Info($"Removed {Data.OrderId}");
                MarkAsComplete();
                return;
            }

            var timenow = DateTime.UtcNow;
            var timeForNextTimeout = Data.LastUpdated.Add(MaxLifeTimeSaga);
            await RequestTimeout(context, timeForNextTimeout, new RemoveSagaTimeout() { timeSent = timenow });
            //log.Info($"Extended {Data.OrderId}");
        }

        /// <summary>
        /// Saga state
        /// </summary>
        public class MySagaData : ContainSagaData
        {
            public Guid OrderId { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        public class RemoveSagaTimeout
        {
            public DateTime timeSent;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
            mapper.ConfigureMapping<PlaceOrder>(m => m.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<UpdateOrder>(m => m.OrderId).ToSaga(s => s.OrderId);
        }
    }
}