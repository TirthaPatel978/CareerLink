using CareerLink.Data;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),

                JobSeekers = await _context.JobSeekers.CountAsync(),

                Recruiters = await _context.Recruiters.CountAsync(),

                Companies = await _context.Companies.CountAsync(),

                Jobs = await _context.Jobs.CountAsync(),

                ActiveJobs = await _context.Jobs
                    .CountAsync(x => x.IsActive),

                Applications = await _context.Applications.CountAsync(),

                Interviews = await _context.Interviews.CountAsync(),

                ActiveUsers = await _context.Users
                    .CountAsync(x => x.IsActive),

                InactiveUsers = await _context.Users
                    .CountAsync(x => !x.IsActive)
            };

            return View(model);
        }
    }
}