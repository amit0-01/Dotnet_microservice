using System.ComponentModel.DataAnnotation;
using System.ComponentModel.DataAnnotation.Schema;

namespace OrderService.Models;

pubic class OrderItem {
    [key]
    public int Id {get; set;}
    public int ProductId {get; set;}
    pubic int Quantity {get; set;}
    pubic decimal Price {get; set;}

    [ForeignKey("Order")]
    pubic int OrderId {get; set;}
    pubic Order Order {get;set;}
}