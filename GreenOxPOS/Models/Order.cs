using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenOxPOS.Models
{
    public class Order
    {
        public long id { get; set; }
        public Address Address { get; set; }
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string ProductAmount { get; set; }
        public float DiscountAmount { get; set; }
        public float Payment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }



    }
    public class Customer
    {
        public long Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
    }
    public class Address
    {
        public long Id { get; set; }
        public Customer Customer { get; set; }
        [Display(Name="Address")]
        public string CustAddress { get; set; }

    }
}