using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    public Order Order { get; set; }
}