using CareerLink.Data;
using CareerLink.Models;
using CareerLink.Models.Enums;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Applications
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var applications = await _context.Applications
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .OrderByDescending(x => x.AppliedAt)
                .ToListAsync();

            return View(applications);
        }

        // GET: /Applications/Create/5
        [HttpGet]
        public async Task<IActionResult> Create(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var job = await _context.Jobs
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x =>
                    x.Id == jobId &&
                    x.IsActive);

            if (job == null)
            {
                return NotFound("Job was not found.");
            }

            if (job.ApplicationDeadline.HasValue &&
                job.ApplicationDeadline.Value < DateTime.UtcNow)
            {
                TempData["ErrorMessage"] =
                    "The application deadline for this job has passed.";

                return RedirectToAction(
                    "Details",
                    "Jobs",
                    new { id = jobId });
            }

            var alreadyApplied = await _context.Applications
                .AnyAsync(x =>
                    x.JobId == jobId &&
                    x.JobSeekerId == jobSeeker.Id);

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] =
                    "You have already applied for this job.";

                return RedirectToAction(
                    "Details",
                    "Jobs",
                    new { id = jobId });
            }

            var model = new ApplicationViewModel
            {
                JobId = job.Id,
                JobTitle = job.Title,
                CompanyName = job.Company.Name,
                JobLocation = job.Location,
                ApplicationDeadline = job.ApplicationDeadline
            };

            return View(model);
        }

        // POST: /Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ApplicationViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var job = await _context.Jobs
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x =>
                    x.Id == model.JobId &&
                    x.IsActive);

            if (job == null)
            {
                return NotFound("Job was not found.");
            }

            if (job.ApplicationDeadline.HasValue &&
                job.ApplicationDeadline.Value < DateTime.UtcNow)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The application deadline for this job has passed.");

                model.JobTitle = job.Title;
                model.CompanyName = job.Company.Name;
                model.JobLocation = job.Location;
                model.ApplicationDeadline =
                    job.ApplicationDeadline;

                return View(model);
            }

            var alreadyApplied = await _context.Applications
                .AnyAsync(x =>
                    x.JobId == model.JobId &&
                    x.JobSeekerId == jobSeeker.Id);

            if (alreadyApplied)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "You have already applied for this job.");

                model.JobTitle = job.Title;
                model.CompanyName = job.Company.Name;
                model.JobLocation = job.Location;
                model.ApplicationDeadline =
                    job.ApplicationDeadline;

                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.JobTitle = job.Title;
                model.CompanyName = job.Company.Name;
                model.JobLocation = job.Location;
                model.ApplicationDeadline =
                    job.ApplicationDeadline;

                return View(model);
            }

            var application = new Application
            {
                JobId = job.Id,
                JobSeekerId = jobSeeker.Id,
                CoverLetter = string.IsNullOrWhiteSpace(
                    model.CoverLetter)
                    ? null
                    : model.CoverLetter.Trim(),
                ResumePath = null,
                MatchScore = null,
                Status = ApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your application was submitted successfully.";

            return RedirectToAction(
                nameof(Details),
                new { id = application.Id });
        }

        // GET: /Applications/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var application = await _context.Applications
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: /Applications/Withdraw/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var application = await _context.Applications
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (application == null)
            {
                return NotFound();
            }

            if (application.Status == ApplicationStatus.Withdrawn)
            {
                TempData["ErrorMessage"] =
                    "This application has already been withdrawn.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            if (application.Status == ApplicationStatus.Selected ||
                application.Status == ApplicationStatus.Rejected)
            {
                TempData["ErrorMessage"] =
                    "This application can no longer be withdrawn.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            application.Status = ApplicationStatus.Withdrawn;
            application.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your application has been withdrawn.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
    }
}