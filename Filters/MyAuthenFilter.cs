
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PBL3Hos.Filters
{
    public class MyAuthenFilter : IAuthorizationFilter
    {
      
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.User.IsInRole("Admin")==false)
            {
                filterContext.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            }
        }

      
    }
}
