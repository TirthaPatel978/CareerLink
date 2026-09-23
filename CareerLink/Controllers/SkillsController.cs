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
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SkillsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Skills
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

            var skills = await _context.JobSeekerSkills
                .Include(x => x.Skill)
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .OrderBy(x => x.Skill.Name)
                .Select(x => new JobSeekerSkillViewModel
                {
                    SkillId = x.SkillId,
                    SkillName = x.Skill.Name,
                    ProficiencyLevel = x.ProficiencyLevel
                })
                .ToListAsync();

            return View(skills);
        }

        // GET: /Skills/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View(new JobSeekerSkillViewModel
            {
                ProficiencyLevel = 1
            });
        }

        // POST: /Skills/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            JobSeekerSkillViewModel model)
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

            // Remove extra spaces from the skill name.
            var skillName = model.SkillName.Trim();

            if (string.IsNullOrWhiteSpace(skillName))
            {
                ModelState.AddModelError(
                    nameof(model.SkillName),
                    "Please enter a skill name.");

                return View(model);
            }

            // Look for an existing skill without caring about
            // uppercase/lowercase differences.
            var skill = await _context.Skills
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == skillName.ToLower());

            // If the skill does not exist, create it.
            if (skill == null)
            {
                skill = new Skill
                {
                    Name = skillName
                };

                _context.Skills.Add(skill);

                await _context.SaveChangesAsync();
            }

            // Check whether this Job Seeker already has this skill.
            var alreadyAdded = await _context.JobSeekerSkills
                .AnyAsync(x =>
                    x.JobSeekerId == jobSeeker.Id &&
                    x.SkillId == skill.Id);

            if (alreadyAdded)
            {
                ModelState.AddModelError(
                    nameof(model.SkillName),
                    "You have already added this skill.");

                return View(model);
            }

            var jobSeekerSkill = new JobSeekerSkill
            {
                JobSeekerId = jobSeeker.Id,
                SkillId = skill.Id,
                ProficiencyLevel = model.ProficiencyLevel
            };

            _context.JobSeekerSkills.Add(jobSeekerSkill);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Skill added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Skills/Edit/5
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

            var skill = await _context.JobSeekerSkills
                .Include(x => x.Skill)
                .FirstOrDefaultAsync(x =>
                    x.JobSeekerId == jobSeeker.Id &&
                    x.SkillId == id);

            if (skill == null)
            {
                return NotFound();
            }

            var model = new JobSeekerSkillViewModel
            {
                SkillId = skill.SkillId,
                SkillName = skill.Skill.Name,
                ProficiencyLevel = skill.ProficiencyLevel
            };

            return View(model);
        }

        // POST: /Skills/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            JobSeekerSkillViewModel model)
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

            var skill = await _context.JobSeekerSkills
                .Include(x => x.Skill)
                .FirstOrDefaultAsync(x =>
                    x.JobSeekerId == jobSeeker.Id &&
                    x.SkillId == model.SkillId);

            if (skill == null)
            {
                return NotFound();
            }

            skill.ProficiencyLevel = model.ProficiencyLevel;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Skill updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Skills/Delete/5
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

            var skill = await _context.JobSeekerSkills
                .Include(x => x.Skill)
                .FirstOrDefaultAsync(x =>
                    x.JobSeekerId == jobSeeker.Id &&
                    x.SkillId == id);

            if (skill == null)
            {
                return NotFound();
            }

            var model = new JobSeekerSkillViewModel
            {
                SkillId = skill.SkillId,
                SkillName = skill.Skill.Name,
                ProficiencyLevel = skill.ProficiencyLevel
            };

            return View(model);
        }

        // POST: /Skills/Delete
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

            var skill = await _context.JobSeekerSkills
                .FirstOrDefaultAsync(x =>
                    x.JobSeekerId == jobSeeker.Id &&
                    x.SkillId == id);

            if (skill == null)
            {
                return NotFound();
            }

            _context.JobSeekerSkills.Remove(skill);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Skill removed successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}