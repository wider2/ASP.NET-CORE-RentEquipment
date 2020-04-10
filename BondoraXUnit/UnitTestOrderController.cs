using System;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Bondora2.Controllers;
using Microsoft.Extensions.Localization;
using Bondora.Api;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace BondoraXUnit
{
    public class UnitTestOrderController
    {
        private OrderController controller;
        private String token;
        private IMemoryCache memoryCache;
     
        public UnitTestOrderController()
        {
            Token t = new Token();
            token = t.CreateToken();
            int customerId = 1;

            ModelParameters model = new ModelParameters();
            model.Token = token;
            model.CustomerId = customerId;

            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            memoryCache = serviceProvider.GetService<IMemoryCache>();
            memoryCache.Set("globalParams", model);

        }

        [Fact]
        public async void TestOrderWithRandomToken()
        {
            var mockRepository = new Mock<IApiOrderRepository>();
            var mockCustomerRepository = new Mock<IApiCustomerRepository>();

            var mockLocalizer = new Mock<IStringLocalizer<SharedResources>>();
            mockLocalizer.Setup(func => func.WithCulture(new CultureInfo("en-GB")));
            
            var mockLogger = new Mock<ILogger<OrderController>>();
            

            controller = new OrderController(null, mockLogger.Object, mockRepository.Object, mockCustomerRepository.Object, memoryCache);

            var result = await controller.Order(token) as ViewResult;

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ViewResult>(viewResult);
            Assert.Null(model.Model);
        }


    }
}