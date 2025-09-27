using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers;

public class TopicController : Controller
{
    private readonly ApplicationDbContext _db;
    private const int PageSize = 10;

    public TopicController(ApplicationDbContext db)
    {
        _db = db;
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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

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
            AuthorName = "TEMP_USER"
        };

        _db.Topics.Add(topic);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Details(int id)
    {
        var topic = _db.Topics.FirstOrDefault(t => t.Id == id);
        if (topic == null) return NotFound();
        return View(topic);
    }
}