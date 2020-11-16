using System.ComponentModel.DataAnnotations;

namespace Inventorizer_Models.Models
{
    public class Category
    {
        [Key]
        public int Category_Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}