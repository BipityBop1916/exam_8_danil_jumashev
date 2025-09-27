using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Forum.Controllers;

public class TopicController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private const int PageSize = 10;

    public TopicController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public IActionResult Index(int page = 1)
    {
        var query = _db.Topics.OrderByDescending(t => t.CreatedAt);
        var total = query.Count();
        var items = query.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        ViewBag.Page = page;
        ViewBag.Total = total;
        ViewBag.PageSize = PageSize;

        return View(items);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Create(string Title, string Content)
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
        {
            ModelState.AddModelError("", "Title and Content are required.");
            return View();
        }

        var topic = new Topic
        {
            Title = Title.Trim(),
            Content = Content.Trim(),
            AuthorName = User.Identity!.Name ?? "Unknown"
        };

        _db.Topics.Add(topic);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var topic = _db.Topics
            .Include(t => t.Replies)
            .ThenInclude(r => r.User)
            .FirstOrDefault(t => t.Id == id);

        if (topic == null) return NotFound();

        return View(topic);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PostReply(int topicId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return BadRequest("Content cannot be empty.");

        var userId = _userManager.GetUserId(User);

        var reply = new Reply
        {
            TopicId = topicId,
            Content = content.Trim(),
            UserId = userId
        };

        _db.Replies.Add(reply);
        await _db.SaveChangesAsync();

        await _db.Entry(reply).Reference(r => r.User).LoadAsync();
        return PartialView("_ReplyPartial", reply);
    }

}