using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Sender";
            var endpointConfiguration = Configuration.GetConfiguration("Sender");

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            #region Press [S] to send message
            Console.WriteLine("Press [S] to send a message.");
            Console.WriteLine("Press [ESC] to exit.");

            var key = new ConsoleKeyInfo();
            while (key.Key != ConsoleKey.Escape)
            {
                key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.S:
                        await SendMessage(endpointInstance);
                        break;
                }
            }
            #endregion

            await endpointInstance.Stop();
        }

        private static async Task SendMessage(IEndpointInstance endpointInstance)
        {
            var id = Guid.NewGuid().ToString().Substring(0, 8);
            Console.WriteLine($"Sending MyMessage with id {id}");
            await endpointInstance.Send(new MyMessage() {Identifier = id}).ConfigureAwait(false);
        }
    }
}
