using System.ComponentModel.DataAnnotations;

namespace UOrders.EFModel
{
    public class LocalizedText : ICloneable
    {
        #region Public Constructors

        public LocalizedText()
        {
            Lang = "en";
            Text = string.Empty;
        }

        public LocalizedText(string lang, string text)
            : this()
        {
            Text = text;
            Lang = lang;
        }

        #endregion Public Constructors

        #region Public Properties

        [Key]
        public virtual int ID { get; set; }

        public virtual string Lang { get; set; }

        public virtual MenuCategory? MenuCategoryDescription { get; set; }

        public virtual MenuCategory? MenuCategoryTitle { get; set; }

        public virtual MenuItem? MenuItemDescription { get; set; }

        public virtual MenuItem? MenuItemTitle { get; set; }

        public virtual MenuItemOption? MenuItemOptionDescription { get; set; }

        public virtual MenuItemOption? MenuItemOptionName { get; set; }

        public virtual MenuItemOptionValue? MenuItemOptionValueName { get; set; }

        public virtual string Text { get; set; }

        #endregion Public Properties

        #region Public Methods

        public object Clone() => new LocalizedText(Lang, Text);

        #endregion Public Methods
    }
}