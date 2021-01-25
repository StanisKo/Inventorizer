using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventorizer_Models.Models
{
    public class Item
    {
        [Key]
        public int Item_Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        /*
        PurchaseDate is not required since it is oftentimes
        harder to recall time of purchase rather than price
        */
        public DateTime PurchaseDate { get; set; }

        public ItemDetail ItemDetail { get; set; }

        [ForeignKey("Category")]
        public int Category_Id { get; set; }
        public Category Category { get; set; }
    }
}