using System.ComponentModel.DataAnnotations;

namespace AspDotNetCoreWebAPI.Models.Dto
{
    public class ProductCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Sku { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}