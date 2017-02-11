using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenOxPOS.Models
{
    public class User
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class ValidUser
    {
        public static bool IsAuthenticated { get; set; }
        public static string UserName { get; set; }
    }
}