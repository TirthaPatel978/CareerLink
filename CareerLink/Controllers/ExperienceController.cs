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
    public class ExperienceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExperienceController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Experience
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

            var experiences = await _context.Experiences
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new ExperienceViewModel
                {
                    Id = x.Id,
                    JobTitle = x.JobTitle,
                    CompanyName = x.CompanyName,
                    Location = x.Location,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsCurrent = x.IsCurrent,
                    Description = x.Description
                })
                .ToListAsync();

            return View(experiences);
        }

        // GET: /Experience/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ExperienceViewModel
            {
                StartDate = DateTime.Today
            });
        }

        // POST: /Experience/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ExperienceViewModel model)
        {
            if (model.IsCurrent)
            {
                model.EndDate = null;
            }

            if (model.EndDate.HasValue &&
                model.EndDate.Value < model.StartDate)
            {
                ModelState.AddModelError(
                    "EndDate",
                    "End date cannot be earlier than the start date.");
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
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker profile was not found.");
            }

            var experience = new Experience
            {
                JobSeekerId = jobSeeker.Id,
                JobTitle = model.JobTitle,
                CompanyName = model.CompanyName,
                Location = model.Location,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsCurrent = model.IsCurrent,
                Description = model.Description
            };

            _context.Experiences.Add(experience);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Experience added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Experience/Edit/5
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

            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (experience == null)
            {
                return NotFound();
            }

            var model = new ExperienceViewModel
            {
                Id = experience.Id,
                JobTitle = experience.JobTitle,
                CompanyName = experience.CompanyName,
                Location = experience.Location,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                IsCurrent = experience.IsCurrent,
                Description = experience.Description
            };

            return View(model);
        }

        // POST: /Experience/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            ExperienceViewModel model)
        {
            if (model.IsCurrent)
            {
                model.EndDate = null;
            }

            if (model.EndDate.HasValue &&
                model.EndDate.Value < model.StartDate)
            {
                ModelState.AddModelError(
                    "EndDate",
                    "End date cannot be earlier than the start date.");
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
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker profile was not found.");
            }

            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (experience == null)
            {
                return NotFound();
            }

            experience.JobTitle = model.JobTitle;
            experience.CompanyName = model.CompanyName;
            experience.Location = model.Location;
            experience.StartDate = model.StartDate;
            experience.EndDate = model.EndDate;
            experience.IsCurrent = model.IsCurrent;
            experience.Description = model.Description;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Experience updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Experience/Delete/5
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

            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (experience == null)
            {
                return NotFound();
            }

            var model = new ExperienceViewModel
            {
                Id = experience.Id,
                JobTitle = experience.JobTitle,
                CompanyName = experience.CompanyName,
                Location = experience.Location,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                IsCurrent = experience.IsCurrent,
                Description = experience.Description
            };

            return View(model);
        }

        // POST: /Experience/Delete
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
                .FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound("Job Seeker profile was not found.");
            }

            var experience = await _context.Experiences
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (experience == null)
            {
                return NotFound();
            }

            _context.Experiences.Remove(experience);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Experience deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}