using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /AdminUsers
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var users = _userManager.Users
                .OrderBy(x => x.FullName)
                .ToList();

            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive,
                    IsCurrentUser = currentUser != null &&
                                    user.Id == currentUser.Id
                });
            }

            return View(model);
        }

        // POST: /AdminUsers/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User was not found.");

            var currentUser = await _userManager.GetUserAsync(User);

            // Prevent the logged-in Admin from disabling their own account.
            if (currentUser != null && user.Id == currentUser.Id)
            {
                TempData["ErrorMessage"] =
                    "You cannot deactivate your own account.";

                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] =
                    "The account status could not be updated.";

                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = user.IsActive
                ? $"{user.Email} has been activated."
                : $"{user.Email} has been deactivated.";

            return RedirectToAction(nameof(Index));
        }
    }
}