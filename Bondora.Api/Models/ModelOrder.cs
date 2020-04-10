using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class ModelOrder
    {
        public int CustomerId { get; set; }

        public string Token { get; set; }

    }
}
