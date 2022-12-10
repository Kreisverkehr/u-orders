using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UOrders.Api.Extensions;
using UOrders.Api.Services;
using UOrders.DTOModel.V1;
using UOrders.DTOModel.V1.Authentication;
using UOrders.EFModel;

namespace UOrders.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MyController : ControllerBase
    {
        #region Private Fields

        private readonly UOrdersDbContext _dbContext;
        private readonly ILogger<MyController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<UOrdersUser> _userManager;
        private readonly IOrdersRepository _ordersRepository;

        #endregion Private Fields

        #region Public Constructors

        public MyController(ILogger<MyController> logger, UOrdersDbContext dbContext, IMapper mapper, UserManager<UOrdersUser> userManager, IOrdersRepository ordersRepository)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _ordersRepository = ordersRepository;
        }

        #endregion Public Constructors

        #region Public Methods

        [HttpGet("orders/{orderId}")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = UserRoles.User)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOrder(int orderId, string lang)
        {
            if (!_dbContext.Orders.Any()) return NotFound();

            var order = _ordersRepository.GetOrder(orderId, lang);

            return order == null ? NotFound() : Ok(order);
        }

        [HttpGet("orders/")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = UserRoles.User)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderOverviewDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.FindByNameAsync(ControllerContext.HttpContext?.User?.Identity?.Name);

            var orders = this.PreparePagedResponse(
                    from order in _dbContext.Orders
                    where order.OrderedBy == user
                    orderby order.OrderedOn descending
                    select order)
                .ProjectTo<OrderOverviewDTO>(_mapper.ConfigurationProvider)
                .ToList();

            foreach (var order in orders)
            {
                order.TotalPrice = _ordersRepository.GetOrderTotalPrice(order.ID);
            }

            return Ok(orders);
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = UserRoles.User)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
        public async Task<IActionResult> GetUser() =>
            Ok(_mapper.Map<UserDTO>(await _userManager.FindByNameAsync(ControllerContext.HttpContext?.User?.Identity?.Name)));

        [HttpPut]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = UserRoles.User)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchEmail(UserDTO userDto)
        {
            var user = await _userManager.FindByNameAsync(ControllerContext.HttpContext?.User?.Identity?.Name);

            if (userDto?.Email != null && user.Email != userDto.Email)
                user.Email = userDto.Email;

            if (userDto?.Name != null && user.Name != userDto.Name)
                user.Name = userDto.Name;

            if (userDto?.Phone != null && user.PhoneNumber != userDto.Phone)
                user.PhoneNumber = userDto.Phone;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        #endregion Public Methods
    }
}