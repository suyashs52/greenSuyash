using System;

using System.ComponentModel.DataAnnotations;

namespace GreenOxPOS.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string ProductName { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is Required")]
        public decimal Price { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public ProductCategory ProductCategory { get; set; }

    }
    public class ProductCategory
    {
        public int PCId { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Product Category is Required")]
        public string Name { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

    }
}