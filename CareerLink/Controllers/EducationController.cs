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
    public class EducationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EducationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Education
        [HttpGet]
        public async Task<IActionResult> Index()
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

            var educationList = await _context.Educations
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new EducationViewModel
                {
                    Id = x.Id,
                    Degree = x.Degree,
                    FieldOfStudy = x.FieldOfStudy,
                    Institution = x.Institution,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Grade = x.Grade
                })
                .ToListAsync();

            return View(educationList);
        }

        // GET: /Education/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Education/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EducationViewModel model)
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

            var education = new Education
            {
                JobSeekerId = jobSeeker.Id,
                Degree = model.Degree,
                FieldOfStudy = model.FieldOfStudy,
                Institution = model.Institution,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Grade = model.Grade
            };

            _context.Educations.Add(education);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Education record added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Education/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

            var education = await _context.Educations
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (education == null)
            {
                return NotFound();
            }

            var model = new EducationViewModel
            {
                Id = education.Id,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                Institution = education.Institution,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Grade = education.Grade
            };

            return View(model);
        }

        // POST: /Education/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            EducationViewModel model)
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

            var education = await _context.Educations
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (education == null)
            {
                return NotFound();
            }

            education.Degree = model.Degree;
            education.FieldOfStudy = model.FieldOfStudy;
            education.Institution = model.Institution;
            education.StartDate = model.StartDate;
            education.EndDate = model.EndDate;
            education.Grade = model.Grade;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Education record updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Education/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
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

            var education = await _context.Educations
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (education == null)
            {
                return NotFound();
            }

            var model = new EducationViewModel
            {
                Id = education.Id,
                Degree = education.Degree,
                FieldOfStudy = education.FieldOfStudy,
                Institution = education.Institution,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                Grade = education.Grade
            };

            return View(model);
        }

        // POST: /Education/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
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

            var education = await _context.Educations
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (education == null)
            {
                return NotFound();
            }

            _context.Educations.Remove(education);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Education record deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}