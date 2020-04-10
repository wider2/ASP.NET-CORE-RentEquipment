using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class ModelInventory
    {
        public int InventoryId { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }

    }
}
