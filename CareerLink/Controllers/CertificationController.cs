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
    public class CertificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CertificationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Certification
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

            var certifications = await _context.Certifications
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .OrderByDescending(x => x.IssueDate)
                .Select(x => new CertificationViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IssuingOrganization = x.IssuingOrganization,
                    IssueDate = x.IssueDate,
                    ExpiryDate = x.ExpiryDate,
                    CredentialUrl = x.CredentialUrl
                })
                .ToListAsync();

            return View(certifications);
        }

        // GET: /Certification/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CertificationViewModel
            {
                IssueDate = DateTime.Today
            });
        }

        // POST: /Certification/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CertificationViewModel model)
        {
            if (model.ExpiryDate.HasValue &&
                model.ExpiryDate.Value.Date < model.IssueDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.ExpiryDate),
                    "Expiry date cannot be before the issue date.");
            }

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
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var certification = new Certification
            {
                JobSeekerId = jobSeeker.Id,
                Name = model.Name.Trim(),
                IssuingOrganization =
                    string.IsNullOrWhiteSpace(
                        model.IssuingOrganization)
                        ? null
                        : model.IssuingOrganization.Trim(),
                IssueDate = model.IssueDate,
                ExpiryDate = model.ExpiryDate,
                CredentialUrl =
                    string.IsNullOrWhiteSpace(model.CredentialUrl)
                        ? null
                        : model.CredentialUrl.Trim()
            };

            _context.Certifications.Add(certification);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Certification added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Certification/Edit/5
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

            var certification =
                await _context.Certifications
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.JobSeekerId == jobSeeker.Id);

            if (certification == null)
            {
                return NotFound();
            }

            var model = new CertificationViewModel
            {
                Id = certification.Id,
                Name = certification.Name,
                IssuingOrganization =
                    certification.IssuingOrganization,
                IssueDate = certification.IssueDate,
                ExpiryDate = certification.ExpiryDate,
                CredentialUrl = certification.CredentialUrl
            };

            return View(model);
        }

        // POST: /Certification/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            CertificationViewModel model)
        {
            if (model.ExpiryDate.HasValue &&
                model.ExpiryDate.Value.Date < model.IssueDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.ExpiryDate),
                    "Expiry date cannot be before the issue date.");
            }

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
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var certification =
                await _context.Certifications
                    .FirstOrDefaultAsync(x =>
                        x.Id == model.Id &&
                        x.JobSeekerId == jobSeeker.Id);

            if (certification == null)
            {
                return NotFound();
            }

            certification.Name = model.Name.Trim();

            certification.IssuingOrganization =
                string.IsNullOrWhiteSpace(
                    model.IssuingOrganization)
                    ? null
                    : model.IssuingOrganization.Trim();

            certification.IssueDate = model.IssueDate;
            certification.ExpiryDate = model.ExpiryDate;

            certification.CredentialUrl =
                string.IsNullOrWhiteSpace(model.CredentialUrl)
                    ? null
                    : model.CredentialUrl.Trim();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Certification updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Certification/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
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

            var certification =
                await _context.Certifications
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.JobSeekerId == jobSeeker.Id);

            if (certification == null)
            {
                return NotFound();
            }

            var model = new CertificationViewModel
            {
                Id = certification.Id,
                Name = certification.Name,
                IssuingOrganization =
                    certification.IssuingOrganization,
                IssueDate = certification.IssueDate,
                ExpiryDate = certification.ExpiryDate,
                CredentialUrl = certification.CredentialUrl
            };

            return View(model);
        }

        // POST: /Certification/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

            var certification =
                await _context.Certifications
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.JobSeekerId == jobSeeker.Id);

            if (certification == null)
            {
                return NotFound();
            }

            _context.Certifications.Remove(certification);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Certification removed successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}