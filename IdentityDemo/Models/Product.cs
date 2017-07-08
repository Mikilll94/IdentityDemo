using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models
{
    [Table("Products")]
    public class Product
    {
        public int ProductID { get; set; }

        [Required]
        public string Name { get; set; }

        public string ImageName { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string SellerID { get; set; }
    }
}
