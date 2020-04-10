using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class BondoraInventoryTypes
    {
        [Key]
        public int TypeId { get; set; }

        public string TypeName { get; set; }

    }
}
