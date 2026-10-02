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
    public class ResumeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".docx"
        };

        public ResumeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: /Resume
        public async Task<IActionResult> Index()
        {
            var jobSeeker = await GetCurrentJobSeekerAsync();

            if (jobSeeker == null)
            {
                return NotFound();
            }

            var model = new ResumeViewModel
            {
                HasResume = !string.IsNullOrWhiteSpace(jobSeeker.ResumePath),
                ExistingFileName = jobSeeker.ResumeFileName
            };

            return View(model);
        }

        // POST: /Resume/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(ResumeViewModel model)
        {
            if (model.ResumeFile == null ||
                model.ResumeFile.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(model.ResumeFile),
                    "Please select a resume file.");

                return View("Index", await BuildViewModelAsync());
            }

            if (model.ResumeFile.Length > MaxFileSize)
            {
                ModelState.AddModelError(
                    nameof(model.ResumeFile),
                    "The resume file must be 5 MB or smaller.");

                return View("Index", await BuildViewModelAsync());
            }

            var extension =
                Path.GetExtension(model.ResumeFile.FileName)
                    .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    nameof(model.ResumeFile),
                    "Only PDF and DOCX files are allowed.");

                return View("Index", await BuildViewModelAsync());
            }

            if (!await IsValidFileSignatureAsync(
                    model.ResumeFile,
                    extension))
            {
                ModelState.AddModelError(
                    nameof(model.ResumeFile),
                    "The uploaded file does not appear to be a valid PDF or DOCX file.");

                return View("Index", await BuildViewModelAsync());
            }

            var jobSeeker = await GetCurrentJobSeekerAsync();

            if (jobSeeker == null)
            {
                return NotFound();
            }

            var resumeDirectory = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "Resumes");

            Directory.CreateDirectory(resumeDirectory);

            // Delete the previous resume if one exists.
            DeleteExistingResume(jobSeeker, resumeDirectory);

            // Generate a safe random filename.
            var storedFileName =
                $"{Guid.NewGuid():N}{extension}";

            var filePath =
                Path.Combine(
                    resumeDirectory,
                    storedFileName);

            await using (var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await model.ResumeFile.CopyToAsync(stream);
            }

            jobSeeker.ResumePath = storedFileName;

            jobSeeker.ResumeFileName =
                Path.GetFileName(model.ResumeFile.FileName);

            jobSeeker.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your resume has been uploaded successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Resume/Download
        [HttpGet]
        public async Task<IActionResult> Download()
        {
            var jobSeeker = await GetCurrentJobSeekerAsync();

            if (jobSeeker == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(jobSeeker.ResumePath))
            {
                return NotFound();
            }

            var resumeDirectory = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "Resumes");

            var safeFileName =
                Path.GetFileName(jobSeeker.ResumePath);

            var filePath =
                Path.Combine(
                    resumeDirectory,
                    safeFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType =
                GetContentType(safeFileName);

            var downloadName =
                string.IsNullOrWhiteSpace(jobSeeker.ResumeFileName)
                    ? safeFileName
                    : jobSeeker.ResumeFileName;

            var fileBytes =
                await System.IO.File.ReadAllBytesAsync(filePath);

            return File(
                fileBytes,
                contentType,
                downloadName);
        }

        // POST: /Resume/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete()
        {
            var jobSeeker = await GetCurrentJobSeekerAsync();

            if (jobSeeker == null)
            {
                return NotFound();
            }

            var resumeDirectory = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "Resumes");

            DeleteExistingResume(jobSeeker, resumeDirectory);

            jobSeeker.ResumePath = null;
            jobSeeker.ResumeFileName = null;
            jobSeeker.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Your resume has been removed.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<JobSeeker?> GetCurrentJobSeekerAsync()
        {
            var userId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _context.JobSeekers
                .FirstOrDefaultAsync(
                    x => x.ApplicationUserId == userId);
        }

        private async Task<ResumeViewModel>
            BuildViewModelAsync()
        {
            var jobSeeker =
                await GetCurrentJobSeekerAsync();

            return new ResumeViewModel
            {
                HasResume =
                    jobSeeker != null &&
                    !string.IsNullOrWhiteSpace(
                        jobSeeker.ResumePath),

                ExistingFileName =
                    jobSeeker?.ResumeFileName
            };
        }

        private void DeleteExistingResume(
            JobSeeker jobSeeker,
            string resumeDirectory)
        {
            if (string.IsNullOrWhiteSpace(
                    jobSeeker.ResumePath))
            {
                return;
            }

            var safeFileName =
                Path.GetFileName(
                    jobSeeker.ResumePath);

            var existingFile =
                Path.Combine(
                    resumeDirectory,
                    safeFileName);

            if (System.IO.File.Exists(existingFile))
            {
                System.IO.File.Delete(existingFile);
            }
        }

        private static string GetContentType(
            string fileName)
        {
            var extension =
                Path.GetExtension(fileName)
                    .ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",

                ".docx" =>
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

                _ => "application/octet-stream"
            };
        }

        private static async Task<bool>
            IsValidFileSignatureAsync(
                IFormFile file,
                string extension)
        {
            await using var stream =
                file.OpenReadStream();

            var header = new byte[8];

            var bytesRead =
                await stream.ReadAsync(header);

            if (extension == ".pdf")
            {
                // PDF files begin with "%PDF"
                return bytesRead >= 4 &&
                       header[0] == 0x25 &&
                       header[1] == 0x50 &&
                       header[2] == 0x44 &&
                       header[3] == 0x46;
            }

            if (extension == ".docx")
            {
                // DOCX files are ZIP-based and begin with "PK".
                return bytesRead >= 2 &&
                       header[0] == 0x50 &&
                       header[1] == 0x4B;
            }

            return false;
        }
    }
}