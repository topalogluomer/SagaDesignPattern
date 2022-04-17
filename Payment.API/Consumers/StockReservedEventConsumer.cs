using MassTransit;
using Microsoft.Extensions.Logging;
using Shared;
using System.Threading.Tasks;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {

        private readonly IPublishEndpoint _endpoint;

        private readonly ILogger<StockReservedEventConsumer> _logger;

        public StockReservedEventConsumer(IPublishEndpoint endpoint, ILogger<StockReservedEventConsumer> logger)
        {
            _endpoint = endpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var availableBalance = 10000m;

            if (availableBalance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} withdrawn from credit card for user id = {context.Message.BuyerId}.");

                await _endpoint.Publish(new PaymentSuccesedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId=context.Message.OrderId,

                });
            }
            else
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL could not withdrawn for user id= {context.Message.BuyerId}");

                await _endpoint.Publish(new PaymentFailedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "not enough money",
                    OrderItems=context.Message.OrderItems


                }) ;
            }
        }
    }
}
