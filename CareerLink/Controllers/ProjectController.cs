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
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Project
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

            var projects = await _context.Projects
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new ProjectViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Technologies = x.Technologies,
                    ProjectUrl = x.ProjectUrl,
                    GithubUrl = x.GithubUrl,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .ToListAsync();

            return View(projects);
        }

        // GET: /Project/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ProjectViewModel model)
        {
            if (model.StartDate.HasValue &&
                model.EndDate.HasValue &&
                model.EndDate.Value < model.StartDate.Value)
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

            var project = new Project
            {
                JobSeekerId = jobSeeker.Id,
                Name = model.Name,
                Description = model.Description,
                Technologies = model.Technologies,
                ProjectUrl = model.ProjectUrl,
                GithubUrl = model.GithubUrl,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            _context.Projects.Add(project);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Project added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Project/Edit/5
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

            var project = await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (project == null)
            {
                return NotFound();
            }

            var model = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Technologies = project.Technologies,
                ProjectUrl = project.ProjectUrl,
                GithubUrl = project.GithubUrl,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };

            return View(model);
        }

        // POST: /Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            ProjectViewModel model)
        {
            if (model.StartDate.HasValue &&
                model.EndDate.HasValue &&
                model.EndDate.Value < model.StartDate.Value)
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

            var project = await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (project == null)
            {
                return NotFound();
            }

            project.Name = model.Name;
            project.Description = model.Description;
            project.Technologies = model.Technologies;
            project.ProjectUrl = model.ProjectUrl;
            project.GithubUrl = model.GithubUrl;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Project updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Project/Delete/5
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

            var project = await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (project == null)
            {
                return NotFound();
            }

            var model = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Technologies = project.Technologies,
                ProjectUrl = project.ProjectUrl,
                GithubUrl = project.GithubUrl,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            };

            return View(model);
        }

        // POST: /Project/Delete
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

            var project = await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobSeekerId == jobSeeker.Id);

            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Project deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}