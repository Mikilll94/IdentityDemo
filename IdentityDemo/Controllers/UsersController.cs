using IdentityDemo.Data;
using IdentityDemo.Models.UsersViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace IdentityDemo.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext _dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var usersViewModel = new IndexViewModel
            {
                Users = _dbContext.Users.OrderBy(u => u.FirstName).ToList()
            };
            return View(usersViewModel);
        }
    }
}