using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class RecruiterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruiterController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Recruiter/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var recruiter = await _context.Recruiters
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

            if (recruiter == null)
            {
                return NotFound("Recruiter profile was not found.");
            }

            var model = new RecruiterDashboardViewModel
            {
                RecruiterName = user.FullName,
                CompanyName = recruiter.Company?.Name
            };

            if (recruiter.CompanyId.HasValue)
            {
                model.TotalJobs = await _context.Jobs
                    .CountAsync(j => j.CompanyId == recruiter.CompanyId.Value);

                model.ActiveJobs = await _context.Jobs
                    .CountAsync(j =>
                        j.CompanyId == recruiter.CompanyId.Value &&
                        j.IsActive);

                model.ClosedJobs = await _context.Jobs
                    .CountAsync(j =>
                        j.CompanyId == recruiter.CompanyId.Value &&
                        !j.IsActive);

                model.TotalApplications = await _context.Applications
                    .CountAsync(a =>
                        a.Job.CompanyId == recruiter.CompanyId.Value);
            }

            return View(model);
        }

        // GET: /Recruiter/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var recruiter = await _context.Recruiters
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

            if (recruiter == null)
            {
                return NotFound("Recruiter profile was not found.");
            }

            var model = new RecruiterProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber,
                JobTitle = recruiter.JobTitle,
                CompanyId = recruiter.CompanyId,
                CompanyName = recruiter.Company?.Name
            };

            return View(model);
        }

        // POST: /Recruiter/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            RecruiterProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var recruiter = await _context.Recruiters
                .FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

            if (recruiter == null)
            {
                return NotFound("Recruiter profile was not found.");
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.Phone;

            recruiter.JobTitle = model.JobTitle;

            await _userManager.UpdateAsync(user);

            _context.Recruiters.Update(recruiter);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your recruiter profile has been updated.";

            return RedirectToAction(nameof(Profile));
        }
    }
}