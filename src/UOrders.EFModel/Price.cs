using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOrders.EFModel
{
    public class Price : ICloneable
    {
        [Key]
        public virtual int ID { get; set; }

        [Required]
        public virtual DateTimeOffset ValidFrom { get; set; } = default;

        public virtual DateTimeOffset? ValidTo { get; set; } = null;

        public virtual MenuItemOptionValue? MenuItemOptionValue { get; set; }

        public virtual MenuItem? MenuItem { get; set; }

        [Precision(9, 4)]
        public virtual decimal Value { get; set; } = 0;

        public object Clone() => new Price()
        {
            ValidFrom = ValidFrom,
            ValidTo = ValidTo,
            Value = Value
        };
    }
}
