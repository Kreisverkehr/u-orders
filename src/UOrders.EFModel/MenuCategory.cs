using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOrders.EFModel
{
    public class MenuCategory
    {
        #region Public Properties

        [ForeignKey("MenuCategoryDescription")]
        public virtual ICollection<LocalizedText> Description { get; set; } = new List<LocalizedText>();

        [Key]
        public int ID { get; set; }

        public virtual ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();

        [ForeignKey("MenuCategoryTitle")]
        public virtual ICollection<LocalizedText> Title { get; set; } = new List<LocalizedText>();

        public bool ToBeRemoved { get; set; } = false;

        #endregion Public Properties
    }
}