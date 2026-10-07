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
    [Authorize(Roles = "Recruiter")]
    public class RecruiterApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruiterApplicationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /RecruiterApplications?jobId=5
        [HttpGet]
        public async Task<IActionResult> Index(int? jobId)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var query = _context.Applications
                .AsNoTracking()
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .Include(x => x.JobSeeker)
                    .ThenInclude(x => x.ApplicationUser)
                .Where(x => x.Job.CompanyId == companyId.Value);

            if (jobId.HasValue)
            {
                query = query.Where(x => x.JobId == jobId.Value);
            }

            var applications = await query
                .OrderByDescending(x => x.AppliedAt)
                .Select(x => new RecruiterApplicantListViewModel
                {
                    ApplicationId = x.Id,
                    JobId = x.JobId,
                    JobTitle = x.Job.Title,
                    ApplicantName = x.JobSeeker.ApplicationUser.FullName,
                    ApplicantEmail = x.JobSeeker.ApplicationUser.Email ?? string.Empty,
                    ProfessionalTitle = x.JobSeeker.ProfessionalTitle,
                    Location = x.JobSeeker.Location,
                    MatchScore = x.MatchScore,
                    Status = x.Status,
                    AppliedAt = x.AppliedAt
                })
                .ToListAsync();

            ViewBag.JobId = jobId;
            ViewBag.CompanyName = await _context.Companies
                .Where(x => x.Id == companyId.Value)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            return View(applications);
        }

        // GET: /RecruiterApplications/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var application = await _context.Applications
                .AsNoTracking()
                .Include(x => x.Job)
                    .ThenInclude(x => x.Company)
                .Include(x => x.JobSeeker)
                    .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Job.CompanyId == companyId.Value);

            if (application == null)
            {
                return NotFound("Application was not found.");
            }

            var model = new RecruiterApplicantDetailsViewModel
            {
                ApplicationId = application.Id,
                JobId = application.JobId,
                JobTitle = application.Job.Title,
                CompanyName = application.Job.Company.Name,

                ApplicantName =
                    application.JobSeeker.ApplicationUser.FullName,

                ApplicantEmail =
                    application.JobSeeker.ApplicationUser.Email
                    ?? string.Empty,

                ApplicantPhone =
                    application.JobSeeker.Phone,

                ProfessionalTitle =
                    application.JobSeeker.ProfessionalTitle,

                Summary =
                    application.JobSeeker.Summary,

                Location =
                    application.JobSeeker.Location,

                MatchScore =
                    application.MatchScore,

                Status =
                    application.Status,

                AppliedAt =
                    application.AppliedAt,

                UpdatedAt =
                    application.UpdatedAt,

                ResumePath =
                    application.ResumePath,

                CoverLetter =
                    application.CoverLetter
            };

            return View(model);
        }

        // POST: /RecruiterApplications/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            ApplicationStatus status)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            // Recruiters should not be able to mark an application
            // as Withdrawn. Withdrawal belongs to the Job Seeker side.
            var allowedStatuses = new[]
            {
                ApplicationStatus.Applied,
                ApplicationStatus.UnderReview,
                ApplicationStatus.Shortlisted,
                ApplicationStatus.Interview,
                ApplicationStatus.Selected,
                ApplicationStatus.Rejected
            };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest("Invalid application status.");
            }

            var application = await _context.Applications
                .Include(x => x.Job)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Job.CompanyId == companyId.Value);

            if (application == null)
            {
                return NotFound("Application was not found.");
            }

            application.Status = status;
            application.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Application status updated to {GetStatusDisplayName(status)}.";

            return RedirectToAction(
                nameof(Details),
                new { id = application.Id });
        }

        private async Task<int?> GetRecruiterCompanyIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return null;
            }

            var recruiter = await _context.Recruiters
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            return recruiter?.CompanyId;
        }

        private static string GetStatusDisplayName(
            ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Applied => "Applied",
                ApplicationStatus.UnderReview => "Under Review",
                ApplicationStatus.Shortlisted => "Shortlisted",
                ApplicationStatus.Interview => "Interview",
                ApplicationStatus.Selected => "Selected",
                ApplicationStatus.Rejected => "Rejected",
                ApplicationStatus.Withdrawn => "Withdrawn",
                _ => status.ToString()
            };
        }
    }
}