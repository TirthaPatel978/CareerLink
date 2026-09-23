using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobSeekerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /JobSeeker/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker profile was not found.");
            }

            var model = new JobSeekerProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                ProfessionalTitle = jobSeeker.ProfessionalTitle,
                Summary = jobSeeker.Summary,
                Phone = jobSeeker.Phone,
                Location = jobSeeker.Location
            };

            return View(model);
        }

        // POST: /JobSeeker/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            JobSeekerProfileViewModel model)
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

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker profile was not found.");
            }

            // Update Identity user's information
            user.FullName = model.FullName;

            // Update Job Seeker information
            jobSeeker.ProfessionalTitle = model.ProfessionalTitle;
            jobSeeker.Summary = model.Summary;
            jobSeeker.Phone = model.Phone;
            jobSeeker.Location = model.Location;
            jobSeeker.UpdatedAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            _context.JobSeekers.Update(jobSeeker);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your profile has been updated successfully.";

            return RedirectToAction(nameof(Profile));
        }
    }
}