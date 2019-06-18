using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    using NServiceBus;

    public class OrderExpired : IEvent
    {
        public string OrderId { get; set; }
    }
}
