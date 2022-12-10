using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UOrders.DTOModel.V1;
using UOrders.DTOModel.V1.Authentication;
using UOrders.EFModel;

namespace UOrders.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MenuController : ControllerBase
{
    #region Private Fields

    private readonly UOrdersDbContext _dbContext;
    private readonly ILogger<MenuController> _logger;
    private readonly IMapper _mapper;

    #endregion Private Fields

    #region Public Constructors

    public MenuController(ILogger<MenuController> logger, UOrdersDbContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    #endregion Public Constructors

    #region Public Methods

    [HttpPost("categories")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    public async Task<IActionResult> CreateMenuCategory(MenuCategoryCreateDTO menuCategory)
    {
        var cat = _mapper.Map<MenuCategory>(menuCategory);
        await _dbContext.Categories.AddAsync(cat);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMenuCategory), new { catId = cat.ID }, cat.ID);
    }

    [HttpPost("categories/{catId}/items")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMenuItem(MenuItemCreateDTO menuItem, int catId)
    {
        var cat = await _dbContext.Categories.FindAsync(catId);
        if (cat == null) return NotFound();

        var item = _mapper.Map<MenuItem>(menuItem);
        item.Category = cat;
        await _dbContext.MenuItems.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMenuItem), new { catId = item.Category.ID, itemId = item.ID }, item.ID);
    }

    [HttpPost("items/{itemId}/options")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMenuItemOption(int itemId, MenuItemOptionCreateDTO menuItemOption)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();

        var opt = _mapper.Map<MenuItemOption>(menuItemOption);

        item.Options.Add(opt);

        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMenuItemOption), new { itemId, optId = opt.ID }, opt.ID);
    }

    [HttpPost("items/{itemId}/options/{optionId}/values")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateMenuItemOptionValue(int itemId, int optionId, MenuItemOptionValueCreateDTO optionVal)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optionId);
        if (opt == default) return NotFound();

        var val = _mapper.Map<MenuItemOptionValue>(optionVal);
        opt.Values.Add(val);

        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMenuItemOptionValue), new { itemId, optionId, valId = val.ID }, val.ID);
    }

    [HttpDelete("categories/{catId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteMenuCategory(int catId, bool cascade = false)
    {
        var cat = await _dbContext.Categories.FindAsync(catId);

        if (cat == default || cat.ToBeRemoved) return NotFound();
        if (!cascade && cat.Items.Where(i => !i.ToBeRemoved).Any()) return Conflict();

        DoDeleteMenuCategory(cat, cascade);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("items/{itemId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteMenuItem(int itemId, bool cascade)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        if (!cascade && item.Options.Where(o => !o.ToBeRemoved).Any()) return Conflict();

        DoDeleteMenuItem(item, cascade);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("items/{itemId}/options/{optionId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteMenuItemOption(int itemId, int optionId, bool cascade)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optionId);
        if (opt == default) return NotFound();

        if (!cascade && opt.Values.Where(o => !o.ToBeRemoved).Any()) return Conflict();

        DoDeleteMenuItemOption(opt, cascade);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("items/{itemId}/options/{optionId}/values/{valId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMenuItemOptionValue(int itemId, int optionId, int valId)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optionId);
        if (opt == default) return NotFound();
        var val = opt.Values.FirstOrDefault(v => v.ID == valId);
        if (val == default) return NotFound();

        DoDeleteMenuItemOptionValue(val);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("categories/")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MenuCategoryDTO>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult GetCategories(string lang = "")
    {
        if (!_dbContext.Categories.Any()) return NoContent();

        IEnumerable<MenuCategoryDTO> categories = _dbContext.Categories
            .Where(c => !c.ToBeRemoved)
            .ProjectTo<MenuCategoryDTO>(_mapper.ConfigurationProvider, new { lang });
        return Ok(categories);
    }

    [HttpGet("items/")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MenuItemDTO>))]
    public IActionResult GetItems(string lang = "") =>
        Ok(_dbContext.MenuItems
            .Where(c => !c.ToBeRemoved)
            .ProjectTo<MenuItemDTO>(_mapper.ConfigurationProvider, new { lang })
        );

    [HttpGet("categories/{catId}/")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MenuCategoryDetailedDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuCategory(int catId)
    {
        var cat = await _dbContext.Categories.FindAsync(catId);
        if (cat == default || cat.ToBeRemoved) return NotFound();

        return Ok(_mapper.Map<MenuCategoryDetailedDTO>(cat));
    }

    [HttpGet("categories/{catId}/items")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MenuItemDetailedDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuCategoryItems(int catId)
    {
        var cat = await _dbContext.Categories.FindAsync(catId);
        if (cat == default || cat.ToBeRemoved) return NotFound();

        return Ok(_mapper.Map<IEnumerable<MenuItemDetailedDTO>>(cat.Items));
    }

    [HttpGet("items/{itemId}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MenuItemDetailedDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItem(int itemId)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default || item.ToBeRemoved) return NotFound();

        return Ok(_mapper.Map<MenuItemDetailedDTO>(item));
    }

    [HttpGet("items/{itemId}/options/{optId}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MenuItemOptionDetailedDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemOption(int itemId, int optId)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optId);
        if (opt == default) return NotFound();

        return Ok(_mapper.Map<MenuItemOptionDetailedDTO>(opt));
    }

    [HttpGet("items/{itemId}/options/{optionId}/values/{valId}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MenuItemOptionValueDetailedDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMenuItemOptionValue(int itemId, int optionId, int valId)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optionId);
        if (opt == default) return NotFound();
        var val = opt.Values.FirstOrDefault(v => v.ID == valId);
        if (val == default) return NotFound();

        return Ok(_mapper.Map<MenuItemOptionValueDetailedDTO>(val));
    }

    [HttpPatch("items/{itemId}/options/{optionId}/values/{valId}/priceChangeToBase")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemOptionValuePrice(int itemId, int optionId, int valId, [FromBody] decimal newPrice)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optionId);
        if (opt == default) return NotFound();
        var val = opt.Values.FirstOrDefault(v => v.ID == valId);
        if (val == default) return NotFound();

        UpdatePrice(val.PriceChangeToBase, newPrice);

        await _dbContext.SaveChangesAsync();

        return Accepted();
    }

    [HttpPatch("items/{itemId}/currentPrice")]
    [MapToApiVersion("1.0")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemPrice(int itemId, [FromBody] decimal newPrice)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == null) return NotFound();

        UpdatePrice(item.Prices, newPrice);

        await _dbContext.SaveChangesAsync();

        return Accepted();
    }

    [HttpPut("categories/{catId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuCategory(int catId, MenuCategoryUpdateDTO menuCategory)
    {
        var cat = await _dbContext.Categories.FindAsync(catId);
        if (cat == default || cat.ToBeRemoved) return NotFound();

        _mapper.Map(menuCategory, cat);

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("items/{itemId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuItem(int itemId, MenuItemUpdateDTO menuItem)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default || item.ToBeRemoved) return NotFound();

        _mapper.Map(menuItem, item);

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("items/{itemId}/options/{optId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuItemOption(int itemId, int optId, MenuItemOptionUpdateDTO menuItemOption)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optId);
        if (opt == default) return NotFound();

        _mapper.Map(menuItemOption, opt);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("items/{itemId}/options/{optionId}/values/{valId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMenuItemOptionValue(int itemId, int optionId, int valId, MenuItemOptionValueUpdateDTO optionVal)
    {
        var item = await _dbContext.MenuItems.FindAsync(itemId);
        if (item == default) return NotFound();
        var opt = item.Options.FirstOrDefault(o => o.ID == optionId);
        if (opt == default) return NotFound();
        var val = opt.Values.FirstOrDefault(v => v.ID == valId);
        if (val == default) return NotFound();

        _mapper.Map(optionVal, val);

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    #endregion Public Methods

    #region Private Methods

    private static void DoDeleteMenuCategory(MenuCategory cat, bool cascade = false)
    {
        if (!cascade && cat.Items.Where(i => !i.ToBeRemoved).Any()) return;

        if (cascade && cat.Items.Where(i => !i.ToBeRemoved).Any())
            foreach (var item in cat.Items)
                DoDeleteMenuItem(item, cascade);

        cat.ToBeRemoved = true;
    }

    private static void DoDeleteMenuItem(MenuItem item, bool cascade = false)
    {
        if (!cascade && item.Options.Where(o => !o.ToBeRemoved).Any()) return;

        if (cascade && item.Options.Where(o => !o.ToBeRemoved).Any())
            foreach (var opt in item.Options)
                DoDeleteMenuItemOption(opt, cascade);

        item.ToBeRemoved = true;
    }

    private static void DoDeleteMenuItemOption(MenuItemOption opt, bool cascade = false)
    {
        if (!cascade && opt.Values.Where(v => !v.ToBeRemoved).Any()) return;

        if (cascade && opt.Values.Where(v => !v.ToBeRemoved).Any())
            foreach (var val in opt.Values)
                DoDeleteMenuItemOptionValue(val);

        opt.ToBeRemoved = true;
    }

    private static void DoDeleteMenuItemOptionValue(MenuItemOptionValue val)
    {
        val.ToBeRemoved = true;
    }

    private static void UpdatePrice(ICollection<Price> priceCollection, decimal newPrice)
    {
        var currentPrice = priceCollection.FirstOrDefault(p => p.ValidFrom < DateTimeOffset.Now && (!p.ValidTo.HasValue || p.ValidTo.Value >= DateTimeOffset.Now));
        if (currentPrice != null)
        {
            currentPrice.ValidTo = DateTimeOffset.Now;
        }

        var futurePrice = priceCollection.OrderBy(p => p.ValidFrom).FirstOrDefault(p => p.ValidFrom >= DateTimeOffset.Now);

        priceCollection.Add(new Price()
        {
            ValidFrom = DateTimeOffset.Now,
            ValidTo = futurePrice?.ValidFrom,
            Value = newPrice
        });
    }

    #endregion Private Methods
}