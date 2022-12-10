using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UOrders.DTOModel.V1;

public class OrderReviewCreateDTO
{
    #region Private Fields

    private string? _name;

    #endregion Private Fields

    #region Public Properties

    [Required]
    public string? Name
    {
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    [Required]
    public decimal StarRating { get; set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public int FullStarRating
    {
        get => Convert.ToInt32(StarRating);
        set => StarRating = value;
    }

    public string? Text { get; set; }

    #endregion Public Properties
}
