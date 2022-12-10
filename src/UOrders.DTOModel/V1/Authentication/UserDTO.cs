using System.ComponentModel.DataAnnotations;
using UOrders.DTOModel.Properties.Resources;

namespace UOrders.DTOModel.V1.Authentication;

public sealed class UserDTO
{
    #region Private Fields

    private string? _email;
    private string? _name;
    private string? _phone;

    #endregion Private Fields

    #region Public Properties

    [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_mail")]
    public string? Email
    {
        get => _email;
        set => _email = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string Id { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;

    public string? Name
    {
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    [Phone(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_phone")]
    public string? Phone
    {
        get => _phone;
        set => _phone = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string? Username { get; set; }

    #endregion Public Properties
}