using MassTransit;
using Microsoft.Extensions.Logging;
using Order.API.Models;
using Shared;
using System.Threading.Tasks;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {

        private readonly ILogger<PaymentFailedEventConsumer> _logger;
        private readonly AppDbContext _context;

        public PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);


            if (order != null)
            {
                order.Status = OrderStatus.Fail;

                order.FailMessage = context.Message.Message;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order status changed {order.Status} ");
            }
            else
            {
                _logger.LogError($"Id= {context.Message.OrderId} not found");
            }


        }
    }
}
