using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class BondoraCart
    {
        [Key]
        public int Id { get; set; }

        public int InventoryId { get; set; }

        public int Days { get; set; }

        public int Status { get; set; }

        public string Token { get; set; }

    }
}
