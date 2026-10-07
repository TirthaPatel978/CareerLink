using CareerLink.Data;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AdminCompanies
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new AdminCompanyListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Industry = x.Industry,
                    Location = x.Location,
                    Website = x.Website,
                    RecruiterCount = x.Recruiters.Count,
                    JobCount = x.Jobs.Count,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return View(companies);
        }

        // GET: /AdminCompanies/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.Companies
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AdminCompanyDetailsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Website = x.Website,
                    Industry = x.Industry,
                    Location = x.Location,
                    LogoPath = x.LogoPath,
                    CreatedAt = x.CreatedAt,
                    RecruiterCount = x.Recruiters.Count,
                    JobCount = x.Jobs.Count,
                    ActiveJobCount = x.Jobs.Count(job => job.IsActive)
                })
                .FirstOrDefaultAsync();

            if (company == null)
                return NotFound("Company was not found.");

            return View(company);
        }
    }
}