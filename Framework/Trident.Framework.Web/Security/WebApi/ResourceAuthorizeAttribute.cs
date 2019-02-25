using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using Trident.Common;

namespace Trident.Web.Security.WebApi
{
    /// <summary>
    /// Class ResourceAuthorizeAttribute.
    /// Implements the <see cref="Trident.Web.Security.WebApi.RoleAuthorizeAttribute" />
    /// </summary>
    /// <seealso cref="Trident.Web.Security.WebApi.RoleAuthorizeAttribute" />
    public class ResourceAuthorizeAttribute : RoleAuthorizeAttribute
    {
        /// <summary>
        /// Determines whether the specified action context is authorized.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if the specified action context is authorized; otherwise, <c>false</c>.</returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var controller = actionContext.ControllerContext.Controller as ApiController;
            var principal = controller.User as ClaimsPrincipal;

            var hasResourceConfiguration = !string.IsNullOrEmpty(Resource)
                 || !string.IsNullOrEmpty(Actions);
           // var passedBaseCheck = base.IsAuthorized(actionContext);

            var retVal = hasResourceConfiguration
                    && principal.Claims.Any(x => 
                        string.Compare(x.Type, Resource, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0
                        && 
                        string.Compare(x.Value, Actions, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0);

            return retVal;
        }

        /// <summary>
        /// Not used. Setter privatized
        /// </summary>
        /// <value>None</value>
        public new string Roles { get; private set; }

        /// <summary>
        /// Not used. Setter privatized
        /// </summary>
        /// <value>None</value>
        public new string Users { get; private set; }


        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>The resource.</value>
        public string Resource { get; set; }


        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public string Actions { get; set; }



    }
}
