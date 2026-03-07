using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _context;

    public OrderController(OrderDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        order.UserId = userId;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return Ok(orders);
    }
}