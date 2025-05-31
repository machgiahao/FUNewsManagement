using FUNewsManagementSystem.BusinessObject.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NewsManagementMVC.Attributes
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public CustomAuthorizeAttribute(params AccountRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles.Select(r => (int)r).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetInt32("Role");
            Console.Write($"Role {role}");
            if(role == null)
            {
                Console.Write($"Role null");
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }
            if(!_allowedRoles.Contains(role.Value))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            }

        }
    }
}
