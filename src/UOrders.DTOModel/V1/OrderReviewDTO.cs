using System.ComponentModel.DataAnnotations;

namespace UOrders.DTOModel.V1;

public class OrderReviewDTO
{
    #region Private Fields

    private string? _name;

    #endregion Private Fields

    #region Public Properties

    public DateTimeOffset? CreatedOn { get; set; }

    [Required]
    public string? Name
    {
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    [Required]
    public decimal StarRating { get; set; }

    public string? Text { get; set; }

    #endregion Public Properties
}