using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class BondoraCustomer
    {
        [Key]
        public int CustomerId { get; set; }

        public string Username { get; set; }

        public int LoyaltyPoints { get; set; }

    }
}
