using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class RabbitMQQueues
    {
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string StockOrderReservedEventQueueName = "stock-order-reserved-queue";
        public const string OrderPaymentSuccesedEventQueueName = "order-payment-succesed-queue";
        public const string OrderPaymentFailedEventQueueName = "order-payment-failed-queue";
        public const string OrderStockNotReservedEventQueueName = "order-stock-notReserved-queue";
        public const string StockPaymentFailedEventQueueName = "stock-payment-failed-queue";


    }
}
