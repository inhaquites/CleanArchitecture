using CleanArchMVC.Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CleanArchMVC.Application.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is Required")]
        [MinLength(3)]
        [MaxLength(150)]
        [DisplayName("Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "The description is Required")]
        [MinLength(5)]
        [MaxLength(250)]
        [DisplayName("Description")]
        public string Description { get; set; }


        [Required(ErrorMessage = "The price is Required")]
        [Column(TypeName = "decimal(10,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        [DisplayName("Price")]
        public decimal Price { get; set; }


        [Required(ErrorMessage = "The stock is Required")]
        [Range(1, 9999)]
        [DisplayName("Stock")]
        public int Stock { get; set; }


        [MaxLength(250)]
        [DisplayName("Product Image")]
        public string Image { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }


        [DisplayName("Categories")]
        public int CategoryId { get; set; }

    }
}
