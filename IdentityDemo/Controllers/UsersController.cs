using IdentityDemo.Data;
using IdentityDemo.Models.UsersViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IdentityDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext _dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var users = _dbContext.Users
                .OrderBy(u => u.FirstName)
                .Include(u => u.Roles)
                .ToList();
            var usersViewModel = users.Select(u => new UserViewModel()
            {
                Login = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
            });
            return View(usersViewModel);
        }
    }
}