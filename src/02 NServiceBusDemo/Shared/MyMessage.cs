using System;
using NServiceBus;

namespace Shared
{
    public class MyMessage : IMessage
    {
        public string Identifier { get; set; }
    }
}
