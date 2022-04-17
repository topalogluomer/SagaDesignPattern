using System.Collections.Generic;

namespace Order.API.Dtos
{
    public class OrderCreateDto
    {
        public int BuyerId { get; set; }
        public List<OrderItemDto> orderItems { get; set; }
        public PaymentDto payment { get; set; }


    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }

    public class PaymentDto
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
    }
}
