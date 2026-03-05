using System.ComponentModel.DataAnnotation
namespace OrderService.Models;

public class Order {

    [key]
    pubic Int Id {get; set;}
    pubic Int UserId {get; set;}
    pubic DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public string Status {get; set;} = "Pending";
    pubic List<OrderItem> Items {get; set;}
    
}