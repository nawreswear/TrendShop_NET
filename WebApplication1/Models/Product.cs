using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Price { get; set; }
        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }
        public int CategoryId { get; set; }
        //public Category Category { get; set; }
        //public Category Category { get; set; } = new Category();
        public Category? Category { get; set; }
        [Required]
        [Display(Name = "Image :")]
        public string? ImagePath { get; set; }

    }
}
