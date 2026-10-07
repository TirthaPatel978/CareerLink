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
        private readonly IWebHostEnvironment _environment;

        private const long MaxResumeSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedResumeExtensions =
        {
            ".pdf",
            ".doc",
            ".docx"
        };

        public ApplicationsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
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
                ApplicationDeadline =
                    job.ApplicationDeadline
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

            // Check application deadline
            if (job.ApplicationDeadline.HasValue &&
                job.ApplicationDeadline.Value < DateTime.UtcNow)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The application deadline for this job has passed.");

                SetJobInformation(model, job);

                return View(model);
            }

            // Check duplicate application
            var alreadyApplied = await _context.Applications
                .AnyAsync(x =>
                    x.JobId == model.JobId &&
                    x.JobSeekerId == jobSeeker.Id);

            if (alreadyApplied)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "You have already applied for this job.");

                SetJobInformation(model, job);

                return View(model);
            }

            // Validate resume
            if (model.Resume == null ||
                model.Resume.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(model.Resume),
                    "Please upload your resume.");
            }
            else
            {
                if (model.Resume.Length > MaxResumeSize)
                {
                    ModelState.AddModelError(
                        nameof(model.Resume),
                        "Resume file size cannot exceed 5 MB.");
                }

                var extension = Path.GetExtension(
                    model.Resume.FileName)
                    .ToLowerInvariant();

                if (!AllowedResumeExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        nameof(model.Resume),
                        "Only PDF, DOC, and DOCX files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                SetJobInformation(model, job);

                return View(model);
            }

            string? savedResumePath = null;

            try
            {
                // Create wwwroot/uploads/resumes
                // if it does not already exist.
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "resumes");

                Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(
                    model.Resume!.FileName)
                    .ToLowerInvariant();

                // Generate a unique filename.
                var uniqueFileName =
                    $"{Guid.NewGuid():N}{extension}";

                var filePath = Path.Combine(
                    uploadsFolder,
                    uniqueFileName);

                // Save the uploaded resume.
                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await model.Resume.CopyToAsync(stream);
                }

                // Save relative path in database.
                savedResumePath =
                    $"/uploads/resumes/{uniqueFileName}";

                var application = new Application
                {
                    JobId = job.Id,

                    JobSeekerId = jobSeeker.Id,

                    CoverLetter =
                        string.IsNullOrWhiteSpace(
                            model.CoverLetter)
                            ? null
                            : model.CoverLetter.Trim(),

                    ResumePath = savedResumePath,

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
            catch
            {
                // Delete uploaded file if database save fails.
                if (!string.IsNullOrWhiteSpace(savedResumePath))
                {
                    var physicalPath = Path.Combine(
                        _environment.WebRootPath,
                        savedResumePath
                            .TrimStart('/')
                            .Replace(
                                '/',
                                Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }

                ModelState.AddModelError(
                    string.Empty,
                    "There was a problem uploading your resume. Please try again.");

                SetJobInformation(model, job);

                return View(model);
            }
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

        // GET: /Applications/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

            // Finalized applications cannot be edited.
            if (application.Status ==
                    ApplicationStatus.Withdrawn ||
                application.Status ==
                    ApplicationStatus.Selected ||
                application.Status ==
                    ApplicationStatus.Rejected)
            {
                TempData["ErrorMessage"] =
                    "This application can no longer be edited.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            var model = new ApplicationViewModel
            {
                JobId = application.JobId,

                JobTitle = application.Job.Title,

                CompanyName =
                    application.Job.Company.Name,

                JobLocation =
                    application.Job.Location,

                ApplicationDeadline =
                    application.Job.ApplicationDeadline,

                CoverLetter =
                    application.CoverLetter,

                ExistingResumePath =
                    application.ResumePath
            };

            return View(model);
        }

        // POST: /Applications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
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

            // Finalized applications cannot be edited.
            if (application.Status ==
                    ApplicationStatus.Withdrawn ||
                application.Status ==
                    ApplicationStatus.Selected ||
                application.Status ==
                    ApplicationStatus.Rejected)
            {
                TempData["ErrorMessage"] =
                    "This application can no longer be edited.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            // The job cannot be changed.
            model.JobId = application.JobId;

            // Validate replacement resume
            // only when a new file is provided.
            if (model.Resume != null &&
                model.Resume.Length > 0)
            {
                if (model.Resume.Length > MaxResumeSize)
                {
                    ModelState.AddModelError(
                        nameof(model.Resume),
                        "Resume file size cannot exceed 5 MB.");
                }

                var extension = Path.GetExtension(
                    model.Resume.FileName)
                    .ToLowerInvariant();

                if (!AllowedResumeExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        nameof(model.Resume),
                        "Only PDF, DOC, and DOCX files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                SetJobInformation(model, application.Job);

                model.ExistingResumePath =
                    application.ResumePath;

                return View(model);
            }

            string? oldResumePath = null;
            string? newResumePath = null;

            try
            {
                // Update cover letter.
                application.CoverLetter =
                    string.IsNullOrWhiteSpace(
                        model.CoverLetter)
                        ? null
                        : model.CoverLetter.Trim();

                // Replace resume only if a new one
                // was uploaded.
                if (model.Resume != null &&
                    model.Resume.Length > 0)
                {
                    var uploadsFolder = Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "resumes");

                    Directory.CreateDirectory(
                        uploadsFolder);

                    var extension = Path.GetExtension(
                        model.Resume.FileName)
                        .ToLowerInvariant();

                    var uniqueFileName =
                        $"{Guid.NewGuid():N}{extension}";

                    var filePath = Path.Combine(
                        uploadsFolder,
                        uniqueFileName);

                    using (var stream = new FileStream(
                        filePath,
                        FileMode.Create))
                    {
                        await model.Resume.CopyToAsync(stream);
                    }

                    newResumePath =
                        $"/uploads/resumes/{uniqueFileName}";

                    oldResumePath =
                        application.ResumePath;

                    application.ResumePath =
                        newResumePath;
                }

                application.UpdatedAt =
                    DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Delete old resume only after
                // database update succeeds.
                if (!string.IsNullOrWhiteSpace(
                    oldResumePath))
                {
                    var oldPhysicalPath =
                        Path.Combine(
                            _environment.WebRootPath,
                            oldResumePath
                                .TrimStart('/')
                                .Replace(
                                    '/',
                                    Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(
                        oldPhysicalPath))
                    {
                        System.IO.File.Delete(
                            oldPhysicalPath);
                    }
                }

                TempData["SuccessMessage"] =
                    "Your application has been updated successfully.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }
            catch
            {
                // Delete newly uploaded file if
                // something goes wrong.
                if (!string.IsNullOrWhiteSpace(
                    newResumePath))
                {
                    var newPhysicalPath =
                        Path.Combine(
                            _environment.WebRootPath,
                            newResumePath
                                .TrimStart('/')
                                .Replace(
                                    '/',
                                    Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(
                        newPhysicalPath))
                    {
                        System.IO.File.Delete(
                            newPhysicalPath);
                    }
                }

                ModelState.AddModelError(
                    string.Empty,
                    "There was a problem updating your application. Please try again.");

                SetJobInformation(
                    model,
                    application.Job);

                model.ExistingResumePath =
                    application.ResumePath;

                return View(model);
            }
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

            if (application.Status ==
                ApplicationStatus.Withdrawn)
            {
                TempData["ErrorMessage"] =
                    "This application has already been withdrawn.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            if (application.Status ==
                    ApplicationStatus.Selected ||
                application.Status ==
                    ApplicationStatus.Rejected)
            {
                TempData["ErrorMessage"] =
                    "This application can no longer be withdrawn.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            application.Status =
                ApplicationStatus.Withdrawn;

            application.UpdatedAt =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your application has been withdrawn.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        // Helper method used when returning to
        // Create/Edit after validation errors.
        private static void SetJobInformation(
            ApplicationViewModel model,
            Job job)
        {
            model.JobTitle = job.Title;
            model.CompanyName = job.Company.Name;
            model.JobLocation = job.Location;
            model.ApplicationDeadline =
                job.ApplicationDeadline;
        }
    }
}