using System;
using System.Collections.Generic;
using Bondora.Api.Controllers;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Bondora2.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BondoraXUnit
{
    public class UnitTestCartController
    {
        private CartController controller;
        private String token;
        private IMemoryCache memoryCache;

        public UnitTestCartController()
        {
            Token t = new Token();
            this.token = t.CreateToken();

            int customerId = 1;
            
            ModelParameters model = new ModelParameters();
            model.Token = token;
            model.CustomerId = customerId;
            
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            memoryCache = serviceProvider.GetService<IMemoryCache>();
            memoryCache.Set("globalParams", model);

            this.controller = new CartController(null, null, memoryCache);
        }

        [Fact]
        public async void TestCartGetDataWithRandomToken()
        {
            var result = await controller.Cart() as ViewResult;
            Assert.NotNull(result);

            var list = result.Model as List<ModelCart>;
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public async void TestApiPostForOrder()
        {
            ModelAddToCart model = new ModelAddToCart();
            model.InventoryId = 2;
            model.Numdays = 5;
            model.Token = token;

            //add to cart
            ApiInventoryController inventoryController = new ApiInventoryController(null);
            var jsonResult = await inventoryController.Post(model);
            Assert.NotNull(jsonResult);
            Assert.True(jsonResult.Success);

            //submit the order
            var submitted = controller.SubmitOrder();
            Assert.NotNull(submitted);

            //check order
            var mockOrderRepository = new Mock<IApiOrderRepository>();
            var mockLogger = new Mock<ILogger<OrderController>>();
            var mockCustomerRepository = new Mock<IApiCustomerRepository>();
            OrderController orderController = new OrderController(null, mockLogger.Object, mockOrderRepository.Object, mockCustomerRepository.Object, memoryCache);

            var result = await orderController.Order(token) as ViewResult;
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsAssignableFrom<ViewResult>(viewResult);
            Assert.NotNull(modelResult);
                     
        }

    }
}