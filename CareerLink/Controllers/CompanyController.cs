using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Company/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var recruiter = await _context.Recruiters
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

            if (recruiter == null)
            {
                recruiter = new Recruiter
                {
                    ApplicationUserId = user.Id
                };

                _context.Recruiters.Add(recruiter);
                await _context.SaveChangesAsync();
            }

            var model = new CompanyProfileViewModel();

            if (recruiter.Company != null)
            {
                model.Name = recruiter.Company.Name;
                model.Description = recruiter.Company.Description;
                model.Website = recruiter.Company.Website;
                model.Industry = recruiter.Company.Industry;
                model.Location = recruiter.Company.Location;
                model.LogoPath = recruiter.Company.LogoPath;
            }

            return View(model);
        }

        // POST: /Company/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            CompanyProfileViewModel model)
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

            var recruiter = await _context.Recruiters
                .Include(r => r.Company)
                .FirstOrDefaultAsync(r => r.ApplicationUserId == user.Id);

            if (recruiter == null)
            {
                recruiter = new Recruiter
                {
                    ApplicationUserId = user.Id
                };

                _context.Recruiters.Add(recruiter);
            }

            Company company;

            if (recruiter.Company == null)
            {
                company = new Company
                {
                    Name = model.Name,
                    Description = model.Description,
                    Website = model.Website,
                    Industry = model.Industry,
                    Location = model.Location,
                    LogoPath = model.LogoPath
                };

                _context.Companies.Add(company);

                recruiter.Company = company;
            }
            else
            {
                company = recruiter.Company;

                company.Name = model.Name;
                company.Description = model.Description;
                company.Website = model.Website;
                company.Industry = model.Industry;
                company.Location = model.Location;
                company.LogoPath = model.LogoPath;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Company profile has been saved successfully.";

            return RedirectToAction(nameof(Profile));
        }
    }
}