using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Models;
using Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        //service ve data katmanı olusturmadım direkt burda halletmek uzatmamak icin 
        private readonly AppDbContext _context;
        //masstransit kullanarak rabbitmq publish edebilmek icn
        private readonly IPublishEndpoint _publishEndPoint;

        public OrdersController(AppDbContext context, IPublishEndpoint publishEndpoint )
        {
            _context = context;
            _publishEndPoint= publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                Status = OrderStatus.Pending,
                BuyerId = orderCreateDto.BuyerId,
                CreatedDate = DateTime.Now
            };

            orderCreateDto.orderItems.ForEach(x =>
            {
                newOrder.Items.Add(new OrderItem() { Count = x.Count, ProductId = x.ProductId, Price = x.Price });
            })
                ;

            await _context.AddAsync(newOrder);

            await _context.SaveChangesAsync();

            //automapper kullanmadım
            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId = orderCreateDto.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentInformation
                {
                    CardName = orderCreateDto.payment.CardName,
                    CardNumber = orderCreateDto.payment.CardNumber,
                    //fiyatı countla carparak toplam buldurdum burada tuttum.
                    TotalPrice= orderCreateDto.orderItems.Sum(s=>s.Price*s.Count)
                }
            };
            orderCreateDto.orderItems.ForEach(item =>
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage
                {
                    Count = item.Count,
                        ProductId = item.ProductId,

                });
            });


            //rabbitMQ direkt olarak exchange e gönderdim.(stok servisi kuyruk olusturunca yakalıcak).
            await _publishEndPoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
