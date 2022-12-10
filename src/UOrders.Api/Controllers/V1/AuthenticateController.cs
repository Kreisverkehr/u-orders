using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using UOrders.Api.Extensions;
using UOrders.Api.Options;
using UOrders.Api.Services;
using UOrders.DTOModel.V1.Authentication;
using UOrders.EFModel;

namespace UOrders.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthenticateController : ControllerBase
{
    #region Private Fields

    private readonly SymmetricSecurityKey _authSigningKey;
    private readonly IOptions<JwtOptions> _configuration;
    private readonly UOrdersDbContext _dbContext;
    private readonly IEmailBuilder _emailBuilder;
    private readonly IMapper _mapper;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SmtpClient _smtpClient;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly UserManager<UOrdersUser> _userManager;

    #endregion Private Fields

    #region Public Constructors

    public AuthenticateController(
        UserManager<UOrdersUser> userManager,
        RoleManager<IdentityRole> roleManager,
        UOrdersDbContext dbContext,
        IMapper mapper,
        IOptions<JwtOptions> configuration,
        SmtpClient smtpClient,
        IEmailBuilder emailBuilder)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _mapper = mapper;
        _configuration = configuration;
        _smtpClient = smtpClient;
        _emailBuilder = emailBuilder;
        _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Secret));
    }

    #endregion Public Constructors

    #region Public Methods

    [HttpPost]
    [Route("users/{userId}/roles/{roleName}")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRoleToUser(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        await _userManager.AddToRoleAsync(user, roleName);

        return NoContent();
    }

    [HttpGet]
    [Route("userAvailable/{userName}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckUser(string userName) =>
        (await _userManager.FindByNameAsync(userName)) != null ? Ok(false) : Ok(true);

    [HttpGet]
    [Route("me")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var currentUser = await _userManager.FindByNameAsync(ControllerContext.HttpContext.User.Identity?.Name ?? string.Empty);
        if (currentUser != null)
        {
            return Ok(_mapper.Map<UserDTO>(currentUser));
        }

        return NotFound();
    }

    [HttpGet]
    [Route("users")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var users = this.PreparePagedResponse(
                from user in _dbContext.Users
                where !string.IsNullOrWhiteSpace(user.UserName)
                orderby user.UserName ascending
                select user)
            .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
            .ToList();

        foreach (var user in users)
            user.IsAdmin = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(user.Id), UserRoles.Admin);
        return Ok(users);
    }

    [HttpPost]
    [Route("login")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString())
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration.Value.ValidIssuer,
                audience: _configuration.Value.ValidAudience,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return Ok(_tokenHandler.WriteToken(token));
        }
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return Forbid();

        UOrdersUser user = new()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        if (model.Name != null)
            user.Name = model.Name;

        if (model.Email != null)
            user.Email = model.Email;

        if (model.Phone != null)
            user.PhoneNumber = model.Phone;

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = new ModelStateDictionary();
            foreach (var err in result.Errors)
                errors.AddModelError<RegisterDTO>(dto => dto.Password, $"{err.Code}: {err.Description}");
            return UnprocessableEntity(errors);
        }
        await _userManager.AddToRoleAsync(user, UserRoles.User);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost]
    [Route("confirmEmail/{email}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> ConfirmEmail(string email, string token)
    {
        //token = HttpUtility.UrlDecode(token);
        var user = await _userManager.FindByEmailAsync(email);
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if(!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable);
        }

        return Ok();
    }

    [HttpDelete]
    [Route("users/{userId}/roles/{roleName}")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        await _userManager.RemoveFromRoleAsync(user, roleName);

        return NoContent();
    }

    #endregion Public Methods
}