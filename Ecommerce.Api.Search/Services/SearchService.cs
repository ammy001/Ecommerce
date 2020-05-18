using Ecoommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecoommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrderService orderService;
        private readonly IProductService productService;
        private readonly ICustomerService customerService;

        public SearchService(IOrderService orderService, IProductService productService, ICustomerService customerService)
        {
            this.orderService = orderService;
            this.productService = productService;
            this.customerService = customerService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResult)> SearchAsync(int customerId)
        {
            var customersResult = await customerService.GetCustomerAsync(customerId);
            var ordersResult = await orderService.GetOrderAsync(customerId);
            var productsResult = await productService.GetProductAsync();

            if (ordersResult.IsSuccess)
            {
                foreach (var order in ordersResult.orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ?
                                productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name :
                                "Product information is not available";
                    }
                }

                var result = new
                {
                    Customer = customersResult.IsSuccess ?
                              customersResult.Customer :
                              new { Name = "Customer information is not available" },
                    Orders = ordersResult.orders
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
