using FoodApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodApi.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly FoodContext _context;

    public NotificationsController(FoodContext context)
    {
        _context = context;
    }

    // GET /api/notifications/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.SentAt)
            .ToListAsync();

        return Ok(notifications);
    }

    // PATCH /api/notifications/{id}/read
    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return NotFound();

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(notification);
    }

    // PATCH /api/notifications/user/{userId}/read-all
    [HttpPatch("user/{userId}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.ReceiverId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var n in notifications)
            n.IsRead = true;

        await _context.SaveChangesAsync();

        return Ok(new { message = "ทำเครื่องหมายอ่านแล้ว", count = notifications.Count });
    }
}
