using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class ModelCart
    {
        public int InventoryId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }

        public int Days { get; set; }

        public decimal Price { get; set; }

        public string PriceCur { get; set; }

    }
}
