using System;
using NServiceBus;

namespace Messages
{
    public class UpdateOrder : ICommand
    {
        public Guid OrderId { get; set; }
    }
}