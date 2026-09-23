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
    public class SavedJobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SavedJobsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /SavedJobs
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

            var savedJobs = await _context.SavedJobs
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .OrderByDescending(x => x.SavedAt)
                .Select(x => new SavedJobViewModel
                {
                    JobId = x.JobId,
                    Title = x.Job.Title,
                    CompanyName = x.Job.Company.Name,
                    Location = x.Job.Location,
                    JobType = x.Job.JobType,
                    SalaryMin = x.Job.SalaryMin,
                    SalaryMax = x.Job.SalaryMax,
                    IsRemote = x.Job.IsRemote,
                    ApplicationDeadline = x.Job.ApplicationDeadline,
                    SavedAt = x.SavedAt
                })
                .ToListAsync();

            return View(savedJobs);
        }

        // POST: /SavedJobs/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int jobId)
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
                .FirstOrDefaultAsync(x =>
                    x.Id == jobId &&
                    x.IsActive);

            if (job == null)
            {
                return NotFound("Job was not found.");
            }

            var alreadySaved = await _context.SavedJobs
                .AnyAsync(x =>
                    x.JobId == jobId &&
                    x.JobSeekerId == jobSeeker.Id);

            if (!alreadySaved)
            {
                var savedJob = new SavedJob
                {
                    JobId = jobId,
                    JobSeekerId = jobSeeker.Id,
                    SavedAt = DateTime.UtcNow
                };

                _context.SavedJobs.Add(savedJob);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                "Details",
                "Jobs",
                new { id = jobId });
        }

        // POST: /SavedJobs/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int jobId)
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

            var savedJob = await _context.SavedJobs
                .FirstOrDefaultAsync(x =>
                    x.JobId == jobId &&
                    x.JobSeekerId == jobSeeker.Id);

            if (savedJob != null)
            {
                _context.SavedJobs.Remove(savedJob);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                "Details",
                "Jobs",
                new { id = jobId });
        }
    }
}