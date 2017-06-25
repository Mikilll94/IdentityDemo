using System.Collections.Generic;

namespace IdentityDemo.Models.UsersViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}
