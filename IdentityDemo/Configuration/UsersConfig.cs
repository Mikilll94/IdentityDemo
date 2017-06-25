using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IdentityDemo.Configuration
{
    public class UsersConfig
    {
        private UserManager<ApplicationUser> _userManager;

        public UsersConfig(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SeedAsync()
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
