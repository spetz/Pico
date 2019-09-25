using System;

namespace Pico.Orders
{
    public class CreateOrder
    {
        public Guid Id { get; }
        public string Client { get; }

        public CreateOrder(Guid id, string client)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            Client = client;
        }
    }
}