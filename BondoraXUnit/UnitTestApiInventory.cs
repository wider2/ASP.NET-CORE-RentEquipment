using System;
using Bondora.Api.Controllers;
using Bondora.Api.Models;
using Bondora.Api.Tools;
using Xunit;

namespace BondoraXUnit
{
    public class UnitTestApiInventory
    {
        private ApiInventoryController controller;
        private String token;

        public UnitTestApiInventory()
        {
            controller = new ApiInventoryController(null);
            Token t = new Token();
            token = t.CreateToken();
        }

        [Fact]
        public async void TestApiPostInventory()
        {
            ModelAddToCart model = new ModelAddToCart();
            model.InventoryId = 1;
            model.Numdays = 3;
            model.Token = token;

            var jsonResult = await controller.Post(model);
            Assert.NotNull(jsonResult);
            Assert.Equal("", jsonResult.Message);
            Assert.True(jsonResult.Success);

            var result = await controller.Get(token);
            Assert.NotNull(result);
            Assert.Equal(5, result.Item1.Count);
            Assert.Equal(1, result.Item2);
        }

        [Fact]
        public async void TestApiGetInventory()
        {
            var result = await controller.Get(token);
            Assert.NotNull(result);
            Assert.Equal(5, result.Item1.Count);
        }
        
    }
}