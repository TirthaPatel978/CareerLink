using CareerLink.Data;
using CareerLink.Models.Enums;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminReportsViewModel
            {
                // Users
                TotalUsers = await _context.Users.CountAsync(),

                JobSeekers = await _context.JobSeekers.CountAsync(),

                Recruiters = await _context.Recruiters.CountAsync(),

                ActiveUsers = await _context.Users
                    .CountAsync(x => x.IsActive),

                InactiveUsers = await _context.Users
                    .CountAsync(x => !x.IsActive),

                // Companies and Jobs
                TotalCompanies = await _context.Companies.CountAsync(),

                TotalJobs = await _context.Jobs.CountAsync(),

                ActiveJobs = await _context.Jobs
                    .CountAsync(x => x.IsActive),

                ClosedJobs = await _context.Jobs
                    .CountAsync(x => !x.IsActive),

                // Applications
                TotalApplications = await _context.Applications.CountAsync(),

                AppliedApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.Applied),

                UnderReviewApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.UnderReview),

                ShortlistedApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.Shortlisted),

                InterviewApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.Interview),

                SelectedApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.Selected),

                RejectedApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.Rejected),

                WithdrawnApplications = await _context.Applications
                    .CountAsync(x => x.Status == ApplicationStatus.Withdrawn),

                // Interviews
                TotalInterviews = await _context.Interviews.CountAsync(),

                ScheduledInterviews = await _context.Interviews
                    .CountAsync(x => x.Status == InterviewStatus.Scheduled),

                CompletedInterviews = await _context.Interviews
                    .CountAsync(x => x.Status == InterviewStatus.Completed),

                CancelledInterviews = await _context.Interviews
                    .CountAsync(x => x.Status == InterviewStatus.Cancelled),

                RescheduledInterviews = await _context.Interviews
                    .CountAsync(x => x.Status == InterviewStatus.Rescheduled)
            };

            return View(model);
        }
    }
}