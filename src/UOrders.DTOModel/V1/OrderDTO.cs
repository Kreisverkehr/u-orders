using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOrders.DTOModel.V1;

public class OrderDTO
{
    public int ID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; } = string.Empty;
    public DateTimeOffset OrderedOn { get; set; }
    public DateTimeOffset OrderedOnLocal => OrderedOn.ToLocalTime();
    public string? Remarks { get; set; }
    public IEnumerable<OrderItemDTO> OrderedItems { get; set; } = new List<OrderItemDTO>();
    public decimal TotalPrice { get; set; }
    public Guid ReviewToken { get; set; }
    public IEnumerable<OrderReviewDTO> Reviews { get; set; } = new List<OrderReviewDTO>();
}
