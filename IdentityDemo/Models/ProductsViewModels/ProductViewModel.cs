using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemo.Models.ProductsViewModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Zdjęcie")]
        public IFormFile Image { get; set; }

        [Required]
        [Display(Name = "Cena")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Sprzedający")]
        public string SellerFullName { get; set; }

        public string SellerID { get; set; }
    }
}
