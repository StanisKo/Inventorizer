using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventorizer_Models.Models
{
    public class ItemDetail
    {
        [Key]
        public int ItemDetail_Id { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        [ForeignKey("Item")]
        public int Item_Id { get; set; }
        public Item Item;
    }
}