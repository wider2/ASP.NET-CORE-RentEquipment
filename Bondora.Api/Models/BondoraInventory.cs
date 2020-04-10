using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class BondoraInventory
    {
        [Key]
        public int InventoryId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

    }
}
