using System.ComponentModel.DataAnnotations;

namespace ProductManagementWebMVC.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Url { get; set; }

        [Required]
        public string Name { get; set; }
        public string? SkuCode { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public string? ImageLink1 { get; set; }
        public string? ImageLink2 { get; set; }

        [Required]
        public int? Stock { get; set; }
    };
}
