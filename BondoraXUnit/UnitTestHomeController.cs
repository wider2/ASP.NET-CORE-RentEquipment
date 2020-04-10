using System;
using System.Collections.Generic;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Bondora2.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace BondoraXUnit
{
    public class UnitTestHomeController
    {
        private HomeController controller;
        private String token;
        //private IMemoryCache cache;

        public UnitTestHomeController()
        {
            //this.cache = memoryCache;
            var cache = new Mock<IMemoryCache>();
            var mockCustomerRepository = new Mock<IApiCustomerRepository>();
            this.controller = new HomeController(null, null, cache.Object, mockCustomerRepository.Object);
            Token t = new Token();
            this.token = t.CreateToken();
        }

        [Fact]
        public async void TestHome()
        {
            var result = await controller.Index() as ViewResult;
            Assert.NotNull(result);

            var list = result.Model as List<ModelInventory>;
            Assert.Equal(5, list.Count);
        }

    }
}