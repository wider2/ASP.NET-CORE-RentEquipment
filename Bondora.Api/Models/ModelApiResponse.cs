using System.ComponentModel.DataAnnotations;

namespace Bondora.Api.Models
{
    public class ModelApiResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int CountOrdered { get; set; }
                
    }
}
