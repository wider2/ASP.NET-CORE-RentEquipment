using System;
using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class BondoraOrder
    {
        [Key]
        public int OrderId { get; set; }

        public int? CustomerId { get; set; }

        public string Token { get; set; }

        public DateTime DateOrder { get; set; }

    }
}
