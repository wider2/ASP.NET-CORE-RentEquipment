using System;
using Bondora.Api.Controllers;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BondoraXUnit
{
    public class UnitTestApiOrder
    {
        private String token;

        public UnitTestApiOrder()
        {
            Token t = new Token();
            token = t.CreateToken();
        }

        [Fact]
        public async void TestApiOrderWithRandomToken()
        {
            
            var mockRepository = new Mock<IApiOrderRepository>();
            
            ApiOrderController orderController = new ApiOrderController(mockRepository.Object);

            var result = await orderController.Get(token);
            Assert.Null(result);
                        
        }

    }
}
