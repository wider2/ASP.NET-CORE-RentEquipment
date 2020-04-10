using System;
using Bondora.Api.Controllers;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Xunit;

namespace BondoraXUnit
{
    public class UnitTestApiCart
    {
        private ApiCartController controller;
        private String token;

        public UnitTestApiCart()
        {
            controller = new ApiCartController(null);
            Token t = new Token();
            token = t.CreateToken();
        }

        [Fact]
        public async void TestApiPostCart()
        {
            ModelOrder model = new ModelOrder();
            model.CustomerId = 1;
            model.Token = token;

            var jsonResult = await controller.Post(model);
            Assert.NotNull(jsonResult);
            Assert.True(jsonResult.Success);

            var result = await controller.Get(token);
            Assert.NotNull(result);
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async void TestApiGetCart()
        {
            var result = await controller.Get(token);
            Assert.NotNull(result);
            //Assert.Equal(0, result.Count);
        }

        [Fact]
        public async void TestApiDeleteCart()
        {
            var result = await controller.Delete(1, token);
            Assert.NotNull(result);
            Assert.False(result.Success);
        }

    }
}