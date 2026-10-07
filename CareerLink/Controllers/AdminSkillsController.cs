using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSkillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var skills = await _context.Skills
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new AdminSkillListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    JobCount = x.JobSkills.Count,
                    JobSeekerCount = x.JobSeekerSkills.Count
                })
                .ToListAsync();

            return View(skills);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminSkillFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminSkillFormViewModel model)
        {
            model.Name = CleanName(model.Name);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var exists = await _context.Skills
                .AnyAsync(x => x.Name.ToLower() == model.Name.ToLower());

            if (exists)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A skill with this name already exists.");

                return View(model);
            }

            var skill = new Skill
            {
                Name = model.Name
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Skill has been added successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var skill = await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (skill == null)
            {
                return NotFound("Skill was not found.");
            }

            var model = new AdminSkillFormViewModel
            {
                Id = skill.Id,
                Name = skill.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            AdminSkillFormViewModel model)
        {
            model.Name = CleanName(model.Name);

            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var skill = await _context.Skills
                .FirstOrDefaultAsync(x => x.Id == id);

            if (skill == null)
            {
                return NotFound("Skill was not found.");
            }

            var exists = await _context.Skills
                .AnyAsync(x =>
                    x.Id != id &&
                    x.Name.ToLower() == model.Name.ToLower());

            if (exists)
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A skill with this name already exists.");

                return View(model);
            }

            skill.Name = model.Name;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Skill has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _context.Skills
                .FirstOrDefaultAsync(x => x.Id == id);

            if (skill == null)
            {
                return NotFound("Skill was not found.");
            }

            var isUsedByJobs = await _context.JobSkills
                .AnyAsync(x => x.SkillId == id);

            var isUsedByJobSeekers = await _context.JobSeekerSkills
                .AnyAsync(x => x.SkillId == id);

            if (isUsedByJobs || isUsedByJobSeekers)
            {
                TempData["ErrorMessage"] =
                    "This skill cannot be deleted because it is currently being used by jobs or job seekers.";

                return RedirectToAction(nameof(Index));
            }

            _context.Skills.Remove(skill);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Skill has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private static string CleanName(string? name)
        {
            return (name ?? string.Empty).Trim();
        }
    }
}