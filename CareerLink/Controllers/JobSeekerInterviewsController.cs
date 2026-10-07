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
    public class JobSeekerInterviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobSeekerInterviewsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /JobSeekerInterviews
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Your Job Seeker profile was not found.");
            }

            var interviews = await _context.Interviews
                .AsNoTracking()
                .Include(x => x.Application)
                    .ThenInclude(x => x.Job)
                        .ThenInclude(x => x.Company)
                .Where(x =>
                    x.Application.JobSeekerId == jobSeeker.Id)
                .OrderBy(x => x.ScheduledAt)
                .Select(x => new JobSeekerInterviewViewModel
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    JobTitle = x.Application.Job.Title,
                    CompanyName = x.Application.Job.Company.Name,
                    ScheduledAt = x.ScheduledAt,
                    InterviewType = x.InterviewType,
                    MeetingLink = x.MeetingLink,
                    Location = x.Location,
                    Notes = x.Notes,
                    Status = x.Status
                })
                .ToListAsync();

            return View(interviews);
        }
    }
}