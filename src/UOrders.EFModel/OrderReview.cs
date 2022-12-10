using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UOrders.EFModel;

public class OrderReview
{
    [Key]
    public virtual int ID { get; set; }

    [Required]
    public virtual string? Name { get; set; }

    [Required]
    [Precision(2, 1)]
    public virtual decimal? StarRating { get; set; }

    public virtual string? Text { get; set; }

    public virtual DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;

    public virtual Order Order { get; set; } = new();
}
