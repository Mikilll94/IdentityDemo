using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IdentityDemo.Configuration
{
    public class UsersConfig
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UsersConfig(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await AddUsersAsync();
            await AddRolesAsync();
            await AssignUserToRoleAsync("admin", "Admin");
        }

        public async Task AssignUserToRoleAsync(string userName, string role)
        {
            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task AddRolesAsync()
        {
            if ((await _roleManager.RoleExistsAsync("Admin")))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            }
        }

        public async Task AddUsersAsync()
        {
            await AddUserAsync("admin", "", "", "admin@poczta.pl", "admin");
            await AddUserAsync("majaW", "Maja", "Wójtowicz", "maja.wojtowicz@poczta.pl", "test");
            await AddUserAsync("weronikaN", "Weronika", "Nawrocka", "weronika.nawrocka@poczta.pl", "test");
            await AddUserAsync("patrykB", "Patryk", "Brzezinski", "patryk.brzezinski@poczta.pl", "test");
        }

        public async Task AddUserAsync(string login, string firstName, string lastName, string email,
            string password)
        {
            if ((await _userManager.FindByNameAsync(login)) == null)
            {
                await _userManager.CreateAsync(
                    new ApplicationUser()
                    {
                        UserName = login,
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email
                    },
                    password
               );
            }
        }
    }
}
