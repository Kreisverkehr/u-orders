using System.ComponentModel.DataAnnotations;
using UOrders.DTOModel.Properties.Resources;

namespace UOrders.DTOModel.V1.Authentication;

public class RegisterDTO
{
    #region Private Fields

    private string? _email;
    private string? _name;
    private string? _phone;
    private string? _username;

    #endregion Private Fields

    #region Public Properties

    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_confirmpassrequired")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
        ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_passwordsdonotmatch")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_mail")]
    public string? Email
    {
        get => _email;
        set => _email = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string? Name
    {
        get => _name;
        set => _name = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    [DataType(DataType.Password)]
    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_passrequired")]
    [StringLength(maximumLength: 4096, MinimumLength = 6,
        ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_passwordlength")]
    [RegularExpression("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#~€\\-,.])[A-Za-z\\d@$!%*?&#~€\\-,.]*",
        ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_passwordrequirements")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "str_dto_register_err_phone")]
    public string? Phone
    {
        get => _phone;
        set => _phone = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    [Required(ErrorMessage = "User Name is required")]
    [StringLength(255, MinimumLength = 2)]
    public string? Username
    {
        get => _username;
        set => _username = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    #endregion Public Properties
}