using Microsoft.AspNetCore.Http;
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
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [NotMapped]
        [Required]
        [Display(Name = "Zdjęcie")]
        public IFormFile Image { get; set; }

        public string ImagePath { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^-?\d{1,16}?(?:\,\d{0,2})?$", ErrorMessage = "Nieprawidłowa cena")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Sprzedający")]
        public string SellerID { get; set; }
    }
}
