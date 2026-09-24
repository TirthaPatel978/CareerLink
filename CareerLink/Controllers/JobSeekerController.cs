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
        [Authorize(Roles = "JobSeeker")]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .Include(x => x.Skills)
                .Include(x => x.Educations)
                .Include(x => x.Experiences)
                .Include(x => x.Projects)
                .Include(x => x.Certifications)
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var savedJobCount = await _context.SavedJobs
                .CountAsync(x =>
                    x.JobSeekerId == jobSeeker.Id);

            var applicationCount = await _context.Applications
                .CountAsync(x =>
                    x.JobSeekerId == jobSeeker.Id);

            var recentJobs = await _context.Jobs
                .Include(x => x.Company)
                .Where(x => x.IsActive)
                .Where(x =>
                    !x.ApplicationDeadline.HasValue ||
                    x.ApplicationDeadline.Value >= DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .Take(5)
                .Select(x => new JobListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    CompanyName = x.Company.Name,
                    Location = x.Location,
                    JobType = x.JobType,
                    SalaryMin = x.SalaryMin,
                    SalaryMax = x.SalaryMax,
                    IsRemote = x.IsRemote,
                    ApplicationDeadline = x.ApplicationDeadline,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            var model = new JobSeekerDashboardViewModel
            {
                FullName = user.FullName,
                ProfessionalTitle = jobSeeker.ProfessionalTitle,
                Location = jobSeeker.Location,
                Summary = jobSeeker.Summary,

                SkillCount = jobSeeker.Skills.Count,
                EducationCount = jobSeeker.Educations.Count,
                ExperienceCount = jobSeeker.Experiences.Count,
                ProjectCount = jobSeeker.Projects.Count,
                CertificationCount = jobSeeker.Certifications.Count,

                SavedJobCount = savedJobCount,
                ApplicationCount = applicationCount,

                RecentJobs = recentJobs
            };

            return View(model);
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