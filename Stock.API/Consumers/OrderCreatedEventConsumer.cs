using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using Stock.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {

        private readonly AppDbContext _context;
        private ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext context, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            //stokta var mı yok mu check ettim
            var stock = new List<bool>();
            //veritabanındaki stok miktarı sipariş miktarından büyük mü kontrol ettim
            foreach (var item in context.Message.OrderItems)
            {
                stock.Add(await _context.Stocks.AnyAsync(p=>p.ProductId==item.ProductId && p.Count>item.Count));
            }

            if(stock.All(x=>x.Equals(true)))
            {
                foreach(var item in context.Message.OrderItems)
                {
                    var stockCount= await _context.Stocks.FirstOrDefaultAsync(x=>x.ProductId==item.ProductId);

                    if(stockCount!=null)
                    {
                        stockCount.Count -= item.Count;

                    }

                    await _context.SaveChangesAsync();

                }

                _logger.LogInformation($"Product was rezerved for BuyerId: {context.Message.BuyerId}");

                //kuyruk ismimi aldım.
                var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQQueues.StockOrderReservedEventQueueName}"));

                //kuyruga stock reserve eventimi göndericem

                StockReservedEvent stockReservedEvent = new StockReservedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Payment = context.Message.Payment,
                    OrderItems = context.Message.OrderItems

                };
                await sendEndPoint.Send(stockReservedEvent);

            }
            else
            {

                await _publishEndpoint.Publish(new StockNotReservedEvent()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Enough stock does not exist"
                });
                _logger.LogInformation($"Not enough for BuyerId: {context.Message.BuyerId}");



            }
        }
    }
}
