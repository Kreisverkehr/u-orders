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

namespace UOrders.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrdersController : ControllerBase
{
    #region Private Fields

    private readonly UOrdersDbContext _dbContext;
    private readonly ILogger<OrdersController> _logger;
    private readonly IMapper _mapper;
    private readonly IPrintQueue _printQueue;
    private readonly INotifyQueueWriter _notifyQueue;
    private readonly UserManager<UOrdersUser> _userManager;
    private readonly IOrdersRepository _ordersRepository;

    #endregion Private Fields

    #region Public Constructors

    public OrdersController(
        ILogger<OrdersController> logger,
        UOrdersDbContext dbContext,
        IMapper mapper,
        IPrintQueue printQueue,
        INotifyQueueWriter notifyQueue,
        UserManager<UOrdersUser> userManager,
        IOrdersRepository ordersRepository)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
        _printQueue = printQueue;
        _notifyQueue = notifyQueue;
        _userManager = userManager;
        _ordersRepository = ordersRepository;
    }

    #endregion Public Constructors

    #region Public Methods

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
    {
        var dbOrder = _mapper.Map<Order>(order);
        foreach (var item in order.Items)
        {
            var menuItem = _dbContext.MenuItems.Find(item.MenuItemID);
            if (menuItem == null)
                return this.ValidationProblem(new ValidationProblemDetails()
                {
                    Title = "Menuitem not found",
                    Detail = $"Menuitem with id {item.MenuItemID} was not found in the database."
                });

            var dbItem = _mapper.Map<OrderItem>(item);
            dbItem.MenuItem = menuItem;

            foreach (var checkedOptionId in item.CheckedOptions)
            {
                var optionVal = menuItem.Options.SelectMany(o => o.Values).Where(v => v.ID == checkedOptionId).First();
                if (optionVal == null)
                    return this.ValidationProblem(new ValidationProblemDetails()
                    {
                        Title = "Item option value not found",
                        Detail = $"Item option value with id {checkedOptionId} was not found in the database."
                    });
                dbItem.CheckedOptions.Add(new() { OptionValue = optionVal });
            }

            dbOrder.OrderedItems.Add(dbItem);
        }

        _dbContext.Orders.Add(dbOrder);

        // add user to order if request was sent by an authenticated user
        if (this.ControllerContext.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            var user = await _userManager.FindByNameAsync(ControllerContext.HttpContext.User.Identity.Name);
            if (user != null)
                dbOrder.OrderedBy = user;
        }

        _dbContext.SaveChanges();

        await _printQueue.EnqueueOrderAsync(dbOrder.ID);
        await _notifyQueue.EnqueueOrderAsync(dbOrder.ID);

        return CreatedAtAction(nameof(GetOrder), new { orderId = dbOrder.ID }, dbOrder.ID);
    }

    [HttpGet("{orderId}")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrder(int orderId, string lang)
    {
        if (!_dbContext.Orders.Any()) return NotFound();

        var order = _ordersRepository.GetOrder(orderId, lang);

        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost("{orderId}/reviews")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateReview(int orderId, Guid reviewToken, [FromBody] OrderReviewCreateDTO reviewDTO)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);

        // checking if order or it's review token is null
        if (order?.ReviewToken == null) return NotFound();
        if (order.ReviewToken != reviewToken) return Forbid();

        order.Reviews.Add(_mapper.Map<OrderReview>(reviewDTO));

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderOverviewDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrders()
    {
        var orders = this.PreparePagedResponse(
                from order in _dbContext.Orders
                orderby order.OrderedOn descending
                select order
            ).ProjectTo<OrderOverviewDTO>(_mapper.ConfigurationProvider).ToList();

        foreach (var order in orders)
        {
            order.TotalPrice = _ordersRepository.GetOrderTotalPrice(order.ID);
        }

        return Ok(orders);
    }

    #endregion Public Methods
}