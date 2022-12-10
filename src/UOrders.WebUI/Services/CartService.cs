using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using UOrders.DTOModel.V1;

namespace UOrders.WebUI.Services;

public class CartService
{
    #region Public Constructors

    public CartService()
    {
        if (OrderedItems is BindingList<OrderedItem> OrderedItemsBinding)
            OrderedItemsBinding.ListChanged += (o, e) => OnCartChanged();
    }

    #endregion Public Constructors

    #region Public Events

    public event EventHandler? CartChanged;

    #endregion Public Events

    #region Public Properties

    [EmailAddress]
    public string? CustomerEmail { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string CustomerName { get; set; } = string.Empty;

    [Phone]
    public string? CustomerPhone { get; set; }

    public IList<OrderedItem> OrderedItems { get; set; } = new BindingList<OrderedItem>();

    public string? Remarks { get; set; }

    #endregion Public Properties

    #region Public Methods

    public OrderCreateDTO GetOrderCreate() => new()
    {
        CustomerName = CustomerName,
        CustomerEmail = CustomerEmail,
        CustomerPhone = CustomerPhone,
        Remarks = Remarks,
        Items = new List<OrderCreateItemDTO>(
            from itm in OrderedItems
            select new OrderCreateItemDTO()
            {
                Remarks = itm.Remarks,
                Count = itm.Count,
                CheckedOptions = itm.CheckedOptions.ToList(),
                MenuItemID = itm.Item.ID
            }
        )
    };

    public void Reset()
    {
        OrderedItems.Clear();
        CustomerName = String.Empty;
        CustomerPhone = null;
        CustomerEmail = null;
        Remarks = null;
    }

    #endregion Public Methods

    #region Private Methods

    private void OnCartChanged() => CartChanged?.Invoke(this, EventArgs.Empty);

    #endregion Private Methods
}

public class OrderedItem : ICloneable, IEquatable<OrderedItem>
{
    #region Public Constructors

    public OrderedItem(MenuItemDTO itm)
    {
        Item = itm;
        PrefillDicts();
    }

    #endregion Public Constructors

    #region Private Constructors

    private OrderedItem()
    {
        Item = new MenuItemDTO();
    }

    #endregion Private Constructors

    #region Public Properties

    public IEnumerable<int> CheckedOptions =>
        SelectOptions.Values.Union(MultiSelectOptions.Where(kv => kv.Value).Select(kv => kv.Key));

    public int Count { get; set; } = 1;

    public IEnumerable<OrderedItemOption> DisplayOptions =>
        SelectOptionsDisplay.Union(SelectMultipleOptionsDisplay);

    public MenuItemDTO Item { get; private set; }

    public Dictionary<int, bool> MultiSelectOptions { get; private set; } = new();

    public decimal? Price => Count *
                    (Item.CurrentPrice + Item.Options.SelectMany(o => o.Values).Where(v => CheckedOptions.Contains(v.ID)).Select(v => v.PriceChangeToBase).Sum());

    public string Remarks { get; set; } = string.Empty;

    public IEnumerable<OrderedItemOption> SelectMultipleOptionsDisplay =>
        from opt in MultiSelectOptions
        where opt.Value
        group opt by Item.Options.First(o => o.Values.Select(v => v.ID).Contains(opt.Key)) into optGroup
        select new OrderedItemOption()
        {
            Name = optGroup.Key.Name,
            Values = optGroup.Key.Values
                .Where(v => optGroup.Select(kv => kv.Key).Contains(v.ID))
                .Select(v => v.Name)
        };

    public Dictionary<int, int> SelectOptions { get; private set; } = new();

    public IEnumerable<OrderedItemOption> SelectOptionsDisplay =>
        from opt in SelectOptions
        select new OrderedItemOption()
        {
            Name = Item.Options.First(o => o.ID == opt.Key).Name,
            Values = new string[] {
                Item.Options.First(o => o.ID == opt.Key).Values.First(v => v.ID == opt.Value).Name
            }
        };

    #endregion Public Properties

    #region Public Methods

    public object Clone() => new OrderedItem()
    {
        Item = Item,
        MultiSelectOptions = MultiSelectOptions,
        SelectOptions = SelectOptions,
        Remarks = Remarks,
        Count = Count
    };

    public bool Equals(OrderedItem? other) =>
        other != null &&
        Item.Equals(other.Item) &&
        MultiSelectOptions.Count == other.MultiSelectOptions.Count &&
        MultiSelectOptions.SequenceEqual(other.MultiSelectOptions) &&
        SelectOptions.Count == other.SelectOptions.Count &&
        SelectOptions.SequenceEqual(other.SelectOptions) &&
        Remarks == other.Remarks;

    public override bool Equals(object? obj) =>
        obj != null && obj is OrderedItem orderedItem ? Equals(orderedItem) : false;

    public override int GetHashCode() =>
        Item.GetHashCode() |
        MultiSelectOptions.Count.GetHashCode() |
        SelectOptions.Count.GetHashCode() |
        Remarks.GetHashCode();

    public void Reset()
    {
        MultiSelectOptions = new();
        SelectOptions = new();
        PrefillDicts();
        Count = 1;
        Remarks = string.Empty;
    }

    #endregion Public Methods

    #region Private Methods

    private void PrefillDicts()
    {
        foreach (var selOpt in Item.Options.Where(o => o.OptionType == MenuItemOptionTypeDTO.Selection))
            SelectOptions.Add(selOpt.ID, selOpt.Values.OrderBy(v => v.PriceChangeToBase).First().ID);
        foreach (var selOptVal in Item.Options.Where(o => o.OptionType == MenuItemOptionTypeDTO.MultiSelect).SelectMany(o => o.Values))
            MultiSelectOptions.Add(selOptVal.ID, false);
    }

    #endregion Private Methods
}

public class OrderedItemOption
{
    #region Public Properties

    public string Name { get; set; } = string.Empty;

    public IEnumerable<string> Values { get; set; } = new List<string>();

    #endregion Public Properties
}