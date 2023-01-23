using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Receiver
{
    public class MyHandler : IHandleMessages<MyMessage>
    {
        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var shouldThrowError = await ShouldIThrow();

            Console.WriteLine($"I received message {message.Identifier} and might throw exception: {shouldThrowError}");

            // if (shouldThrowError)
            //     throw new Exception("BOOM!");
        }

        #region ShouldIThrow

        static Task<bool> ShouldIThrow()
        {
            if (errorsThrownInARow >= 4)
            {
                errorsThrownInARow = 0;
                return Task.FromResult(false);
            }

            // Fifty-fifty chance to throw each time
            if (Random.Next(5) % 2 == 0)
            {
                errorsThrownInARow++;
                return Task.FromResult(true);
            }

            errorsThrownInARow = 0;
            return Task.FromResult(false);
        }

        static readonly Random Random = new Random();
        static int errorsThrownInARow; // Only works with concurrency 
        #endregion 
    }
}
