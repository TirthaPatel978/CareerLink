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
    public class RecruiterInterviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RecruiterInterviewsController> _logger;

        public RecruiterInterviewsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<RecruiterInterviewsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /RecruiterInterviews
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var interviews = await _context.Interviews
                .AsNoTracking()
                .Include(x => x.Application)
                    .ThenInclude(x => x.Job)
                .Include(x => x.Application)
                    .ThenInclude(x => x.JobSeeker)
                        .ThenInclude(x => x.ApplicationUser)
                .Where(x =>
                    x.Application.Job.CompanyId == companyId.Value)
                .OrderBy(x => x.ScheduledAt)
                .Select(x => new RecruiterInterviewViewModel
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    JobTitle = x.Application.Job.Title,
                    ApplicantName =
                        x.Application.JobSeeker.ApplicationUser.FullName,
                    ApplicantEmail =
                        x.Application.JobSeeker.ApplicationUser.Email
                        ?? string.Empty,
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

        // GET: /RecruiterInterviews/Create?applicationId=5
        [HttpGet]
        public async Task<IActionResult> Create(int applicationId)
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
                .Include(x => x.JobSeeker)
                    .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == applicationId &&
                    x.Job.CompanyId == companyId.Value);

            if (application == null)
            {
                return NotFound("Application was not found.");
            }

            var model = new RecruiterInterviewViewModel
            {
                ApplicationId = application.Id,
                JobTitle = application.Job.Title,
                ApplicantName =
                    application.JobSeeker.ApplicationUser.FullName,
                ApplicantEmail =
                    application.JobSeeker.ApplicationUser.Email
                    ?? string.Empty,
                ScheduledAt = DateTime.Now.AddDays(1),
                InterviewType = InterviewType.Online,
                Status = InterviewStatus.Scheduled
            };

            return View(model);
        }

        // POST: /RecruiterInterviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            RecruiterInterviewViewModel model)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var application = await _context.Applications
                .Include(x => x.Job)
                .Include(x => x.JobSeeker)
                    .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == model.ApplicationId &&
                    x.Job.CompanyId == companyId.Value);

            if (application == null)
            {
                return NotFound("Application was not found.");
            }

            ValidateInterview(model);

            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();

                ViewBag.ValidationErrors = errors;

                foreach (var error in errors)
                {
                    _logger.LogWarning(
                        "Schedule Interview validation error: {ValidationError}",
                        error);
                }

                model.JobTitle = application.Job.Title;

                model.ApplicantName =
                    application.JobSeeker.ApplicationUser.FullName;

                model.ApplicantEmail =
                    application.JobSeeker.ApplicationUser.Email
                    ?? string.Empty;

                return View(model);
            }

            var interview = new Interview
            {
                ApplicationId = application.Id,
                ScheduledAt = model.ScheduledAt,
                InterviewType = model.InterviewType,
                MeetingLink = CleanValue(model.MeetingLink),
                Location = CleanValue(model.Location),
                Notes = CleanValue(model.Notes),
                Status = InterviewStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            _context.Interviews.Add(interview);

            application.Status = ApplicationStatus.Interview;
            application.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Interview scheduled successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /RecruiterInterviews/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var interview = await _context.Interviews
                .AsNoTracking()
                .Include(x => x.Application)
                    .ThenInclude(x => x.Job)
                .Include(x => x.Application)
                    .ThenInclude(x => x.JobSeeker)
                        .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Application.Job.CompanyId == companyId.Value);

            if (interview == null)
            {
                return NotFound("Interview was not found.");
            }

            var model = new RecruiterInterviewViewModel
            {
                Id = interview.Id,
                ApplicationId = interview.ApplicationId,
                JobTitle = interview.Application.Job.Title,
                ApplicantName =
                    interview.Application.JobSeeker.ApplicationUser.FullName,
                ApplicantEmail =
                    interview.Application.JobSeeker.ApplicationUser.Email
                    ?? string.Empty,
                ScheduledAt = interview.ScheduledAt,
                InterviewType = interview.InterviewType,
                MeetingLink = interview.MeetingLink,
                Location = interview.Location,
                Notes = interview.Notes,
                Status = interview.Status
            };

            return View(model);
        }

        // POST: /RecruiterInterviews/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            RecruiterInterviewViewModel model)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var interview = await _context.Interviews
                .Include(x => x.Application)
                    .ThenInclude(x => x.Job)
                .Include(x => x.Application)
                    .ThenInclude(x => x.JobSeeker)
                        .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id &&
                    x.Application.Job.CompanyId == companyId.Value);

            if (interview == null)
            {
                return NotFound("Interview was not found.");
            }

            ValidateInterview(model);

            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();

                ViewBag.ValidationErrors = errors;

                foreach (var error in errors)
                {
                    _logger.LogWarning(
                        "Reschedule Interview validation error: {ValidationError}",
                        error);
                }

                model.ApplicationId = interview.ApplicationId;

                model.JobTitle =
                    interview.Application.Job.Title;

                model.ApplicantName =
                    interview.Application.JobSeeker.ApplicationUser.FullName;

                model.ApplicantEmail =
                    interview.Application.JobSeeker.ApplicationUser.Email
                    ?? string.Empty;

                return View(model);
            }

            interview.ScheduledAt = model.ScheduledAt;

            interview.InterviewType =
                model.InterviewType;

            interview.MeetingLink =
                CleanValue(model.MeetingLink);

            interview.Location =
                CleanValue(model.Location);

            interview.Notes =
                CleanValue(model.Notes);

            interview.Status =
                InterviewStatus.Rescheduled;

            interview.Application.Status =
                ApplicationStatus.Interview;

            interview.Application.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Interview rescheduled successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /RecruiterInterviews/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            InterviewStatus status)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return NotFound(
                    "You are not currently associated with a company.");
            }

            var allowedStatuses = new[]
            {
                InterviewStatus.Scheduled,
                InterviewStatus.Completed,
                InterviewStatus.Cancelled,
                InterviewStatus.Rescheduled
            };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest("Invalid interview status.");
            }

            var interview = await _context.Interviews
                .Include(x => x.Application)
                    .ThenInclude(x => x.Job)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.Application.Job.CompanyId == companyId.Value);

            if (interview == null)
            {
                return NotFound("Interview was not found.");
            }

            interview.Status = status;

            if (status == InterviewStatus.Cancelled)
            {
                interview.Application.Status =
                    ApplicationStatus.Shortlisted;
            }
            else if (status == InterviewStatus.Completed)
            {
                interview.Application.Status =
                    ApplicationStatus.Interview;
            }
            else if (status == InterviewStatus.Scheduled ||
                     status == InterviewStatus.Rescheduled)
            {
                interview.Application.Status =
                    ApplicationStatus.Interview;
            }

            interview.Application.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Interview status updated to {GetStatusDisplayName(status)}.";

            return RedirectToAction(nameof(Index));
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

        private void ValidateInterview(
            RecruiterInterviewViewModel model)
        {
            if (model.ScheduledAt <= DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(model.ScheduledAt),
                    "Interview date and time must be in the future.");
            }

            if (model.InterviewType == InterviewType.Online &&
                string.IsNullOrWhiteSpace(model.MeetingLink))
            {
                ModelState.AddModelError(
                    nameof(model.MeetingLink),
                    "A meeting link is required for an online interview.");
            }

            if (model.InterviewType == InterviewType.InPerson &&
                string.IsNullOrWhiteSpace(model.Location))
            {
                ModelState.AddModelError(
                    nameof(model.Location),
                    "A location is required for an in-person interview.");
            }
        }

        private List<string> GetModelStateErrors()
        {
            return ModelState
                .Where(x =>
                    x.Value != null &&
                    x.Value.Errors.Count > 0)
                .SelectMany(x =>
                    x.Value!.Errors.Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? $"{x.Key}: Invalid value."
                            : $"{x.Key}: {error.ErrorMessage}"))
                .ToList();
        }

        private static string? CleanValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        private static string GetStatusDisplayName(
            InterviewStatus status)
        {
            return status switch
            {
                InterviewStatus.Scheduled => "Scheduled",
                InterviewStatus.Completed => "Completed",
                InterviewStatus.Cancelled => "Cancelled",
                InterviewStatus.Rescheduled => "Rescheduled",
                _ => status.ToString()
            };
        }
    }
}