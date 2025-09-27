using Forum.Data;
using Forum.Models;
using Forum.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(model);
            }

            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                Directory.CreateDirectory(uploadsRoot);

                var fname = $"{Guid.NewGuid():N}{Path.GetExtension(model.Avatar.FileName)}";
                var path = Path.Combine(uploadsRoot, fname);

                using (var fs = new FileStream(path, FileMode.Create))
                    await model.Avatar.CopyToAsync(fs);

                user.AvatarPath = $"/uploads/avatars/{fname}";
                await _userManager.UpdateAsync(user);
            }
            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("Index", "Topic");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            ApplicationUser? user = null;
            if (model.Login.Contains('@'))
                user = await _userManager.FindByEmailAsync(model.Login);
            else
                user = await _userManager.FindByNameAsync(model.Login);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login or password.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Topic");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Topic");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Count topics created by user
            var topicCount = await _context.Topics
                .CountAsync(t => t.AuthorName == user.UserName);

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                AvatarPath = user.AvatarPath ?? "/images/default-avatar.png",
            };

            return View(model);
        }


        public IActionResult AccessDenied() => View();
}