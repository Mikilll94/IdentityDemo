using IdentityDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IdentityDemo.Authorization
{
    public class ProductIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        UserManager<ApplicationUser> _userManager;

        public ProductIsOwnerAuthorizationHandler(UserManager<ApplicationUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   string resource_sellerID)
        {
            if (resource_sellerID == null)
            {
                return Task.FromResult(0);
            }

            if (requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.FromResult(0);
            }

            if (resource_sellerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}