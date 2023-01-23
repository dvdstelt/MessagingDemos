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

            if (shouldThrowError)
                throw new Exception("BOOM!");
        }

        #region ShouldIThrow
        async Task<bool> ShouldIThrow()
        {
            if (errorsThrownInARow >= 4)
            {
                errorsThrownInARow = 0;
                return false;
            }

            // Fifty-fifty chance to throw each time
            if (random.Next(5) % 2 == 0)
            {
                errorsThrownInARow++;
                return true;
            }

            errorsThrownInARow = 0;
            return false;
        }

        static Random random = new Random();
        static int errorsThrownInARow; // Only works with concurrency 
        #endregion 
    }
}
