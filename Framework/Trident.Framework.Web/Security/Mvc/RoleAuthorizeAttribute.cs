using System.Linq;
using System.Web.Mvc;
using System.Security.Claims;
using System.Reflection;

namespace Trident.Web.Security.Mvc
{

    /// <summary>
    /// Class RoleAuthorizeAttribute.
    /// Implements the <see cref="System.Web.Mvc.AuthorizeAttribute" />
    /// </summary>
    /// <seealso cref="System.Web.Mvc.AuthorizeAttribute" />
    public class RoleAuthorizeAttribute:AuthorizeAttribute
    {
        /// <summary>
        /// Authorizes the core.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool AuthorizeCore(AuthorizationContext actionContext)
        {         
            var controller = actionContext.Controller as Controller;
             
            //this will come back once the legacy is gone from security in the meantime we will use 
            //service locator pattern to get the current scoped instance of principal resolver
            var claimsPrincipal = controller.User as ClaimsPrincipal;                       
           
            var identity = claimsPrincipal?.Identities.FirstOrDefault();
            var username = (identity != null) ? identity.Name : string.Empty;
            Serilog.Log.Logger.Information("Principal Type: " + controller.User.GetType().FullName);
            Serilog.Log.Logger.Information("Principal username: " + username);

            bool isAuthorized = claimsPrincipal.Identity.IsAuthenticated;
            var classAttrs = actionContext.ActionDescriptor.ControllerDescriptor.ControllerType.GetCustomAttributes<RoleAuthorizeAttribute>().ToList();
            var methodAttrs = actionContext.ActionDescriptor.GetCustomAttributes(true)
                    .Where(x => typeof(RoleAuthorizeAttribute).IsAssignableFrom(x.GetType()))
                    .Cast<RoleAuthorizeAttribute>().ToList();
          
            classAttrs.ForEach(x =>
            {
                isAuthorized &= x.IsAuthorized(claimsPrincipal);
            });

            methodAttrs.ForEach(x =>
            {
                isAuthorized &= x.IsAuthorized(claimsPrincipal);
            });

            return isAuthorized;
        }

        /// <summary>
        /// Determines whether the specified claims principal is authorized.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <returns><c>true</c> if the specified claims principal is authorized; otherwise, <c>false</c>.</returns>
        private bool IsAuthorized(ClaimsPrincipal claimsPrincipal)
        {
            bool isAuthenticated = claimsPrincipal.Identity.IsAuthenticated;
            if (!isAuthenticated) return false;
            if (string.IsNullOrWhiteSpace(this.Roles)) return isAuthenticated;

            var isAuthorized = false;
            var roles = this.Roles.Split(',').ToList();
            roles.ForEach(r =>  isAuthorized |= claimsPrincipal.IsInRole(r.Trim()));         
            return isAuthorized;
        }


        /// <summary>
        /// Called when a process requests authorization.
        /// </summary>
        /// <param name="filterContext">The filter context, which encapsulates information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute" />.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!this.AuthorizeCore(filterContext))
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
