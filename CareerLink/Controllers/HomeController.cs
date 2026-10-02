using System.Diagnostics;
using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.UtcNow;

            var jobs = await _context.Jobs
                .AsNoTracking()
                .Include(j => j.Company)
                .Include(j => j.Skills)
                    .ThenInclude(js => js.Skill)
                .Where(j =>
                    j.IsActive &&
                    (
                        j.ApplicationDeadline == null ||
                        j.ApplicationDeadline >= currentDate
                    ))
                .OrderByDescending(j => j.CreatedAt)
                .Take(3)
                .ToListAsync();

            var model = new HomeViewModel
            {
                FeaturedJobs = jobs.Select(j => new FeaturedJobViewModel
                {
                    Id = j.Id,
                    Title = j.Title,
                    CompanyName = j.Company.Name,
                    Location = j.Location,
                    IsRemote = j.IsRemote,
                    JobType = j.JobType,
                    SalaryMin = j.SalaryMin,
                    SalaryMax = j.SalaryMax,
                    Skills = j.Skills
                        .OrderByDescending(s => s.IsRequired)
                        .ThenBy(s => s.Skill.Name)
                        .Take(4)
                        .Select(s => s.Skill.Name)
                        .ToList()
                }).ToList()
            };

            return View(model);
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}