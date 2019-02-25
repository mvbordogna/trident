using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Security.Claims;
using System.Net.Http;
using Trident.Security;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Trident.Common;
using System;
using System.Net;
using System.Web.Http.Dependencies;
using Trident.Security.Managers;
using System.Collections.Specialized;
using System.Web;

namespace Trident.Web.Security.WebApi
{

    /// <summary>
    /// Class RoleAuthorizeAttribute.
    /// Implements the <see cref="System.Web.Http.AuthorizeAttribute" />
    /// Implements the <see cref="Autofac.Integration.WebApi.IAutofacAuthorizationFilter" />
    /// </summary>
    /// <seealso cref="System.Web.Http.AuthorizeAttribute" />
    /// <seealso cref="Autofac.Integration.WebApi.IAutofacAuthorizationFilter" />
    public class RoleAuthorizeAttribute : AuthorizeAttribute, Autofac.Integration.WebApi.IAutofacAuthorizationFilter
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; set; }

        /// <summary>
        /// on authorization as an asynchronous operation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="cancellationToken">Converts to ken.</param>
        /// <returns>Task.</returns>
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var hasClassAttrs = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<RoleAuthorizeAttribute>().Any();
            var hasMethodAttrs = actionContext.ActionDescriptor.GetCustomAttributes<RoleAuthorizeAttribute>().Any();

            if ((hasClassAttrs || hasMethodAttrs) && this.Source == "Pipeline")
                return;

            var isValidMobileApiKey = await AssureMobileAppUserKey(actionContext);

            if (!isValidMobileApiKey)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            if (!this.AuthorizeCore(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
            }
        }
        /// <summary>
        /// Assures the mobile application user key.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> AssureMobileAppUserKey(HttpActionContext actionContext)
        {
            IDependencyScope requestScope = CreateDependencyResolver(actionContext);
            var signInManager = requestScope.GetService(typeof(ISignInManager)) as ISignInManager;
            var appSettings = requestScope.GetService(typeof(IAppSettings)) as IAppSettings;         
            Guid userKey = default(Guid);

            if (appSettings == null)
            {
                return false;
            }

            var parameters = actionContext.ActionDescriptor.GetParameters();
            var param = parameters.FirstOrDefault(x => typeof(IMobileAppSecurityKey).IsAssignableFrom(x.ParameterType));
            if (param != null)
            {
                if (actionContext.Request.Method == HttpMethod.Get)
                {
                    var formData = actionContext.Request.RequestUri.ParseQueryString();
                    var (IsValid, ApiKey, UserKey) = GetKeysFromFormData(formData);
                    if (!IsValid)
                    {
                        return false;
                    }
                 
                    userKey = UserKey;
                }
                else if (actionContext.Request.Content.Headers.ContentType?.MediaType == "application/x-www-form-urlencoded")
                {
                    var dataString = await actionContext.Request.Content.ReadAsStringAsync();
                    var formData = HttpUtility.ParseQueryString(dataString);
                    var (IsValid, ApiKey, UserKey) = GetKeysFromFormData(formData);
                    if (!IsValid)
                    {
                        return false;
                    }
                   
                    userKey = UserKey;

                }
                else
                {
                    var dataString = await actionContext.Request.Content.ReadAsStringAsync();
                    var (IsValid, ApiKey, UserKey) = GetKeysFromRequestContent(dataString, param.ParameterType);
                    if (!IsValid)
                    {
                        return false;
                    }
               
                    userKey = UserKey;
                }

            }
            else
            {
                return true;
            }
                     
            return false;
        }

        /// <summary>
        /// Creates the dependency resolver.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>IDependencyScope.</returns>
        protected virtual IDependencyScope CreateDependencyResolver(HttpActionContext actionContext)
        {
            return actionContext.Request.GetDependencyScope();
        }

        /// <summary>
        /// Checks the mobile API key.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="appSettings">The application settings.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckMobileApiKey(Guid apiKey, IAppSettings appSettings)
        {
            return string.Compare(appSettings["api.key"], apiKey.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Authorizes the core.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool AuthorizeCore(HttpActionContext actionContext)
        {//ServerSecurityKeyCheck
            var anonymousClassAttrs = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().ToList();
            var anonymousMethodAttrs = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().ToList();

            var classAttrs = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<RoleAuthorizeAttribute>().ToList();
            var methodAttrs = actionContext.ActionDescriptor.GetCustomAttributes<RoleAuthorizeAttribute>().ToList();


            if ((anonymousMethodAttrs.Any() || anonymousClassAttrs.Any()) && !(classAttrs.Any() || methodAttrs.Any()))
            {
                return true;
            }

            var controller = actionContext.ControllerContext.Controller as ApiController;
            //this will come back once the legacy is gone from security in the meantime we will use 
            //service locator pattern to get the current scoped instance of principal resolver
            var claimsPrincipal = controller.User as ClaimsPrincipal;

            var requestScope = CreateDependencyResolver(actionContext);
            var _principalResolver = requestScope.GetService(typeof(IPrincipalResolver)) as IPrincipalResolver;
            claimsPrincipal = _principalResolver.GetUserPrincipal() as ClaimsPrincipal;

            var identity = claimsPrincipal?.Identities.FirstOrDefault();
            var username = (identity != null) ? identity.Name : string.Empty;
            Serilog.Log.Logger.Information("Principal Type: " + controller.User.GetType().FullName);
            Serilog.Log.Logger.Information("Principal username: " + username);

            bool isAuthorized = base.IsAuthorized(actionContext);


            classAttrs.ForEach(x =>
            {
                isAuthorized &= x.IsAuthorized(actionContext);
            });

            methodAttrs.ForEach(x =>
            {
                isAuthorized &= x.IsAuthorized(actionContext);
            });

            return isAuthorized;
        }

        /// <summary>
        /// Gets the keys from form data.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <returns>System.ValueTuple&lt;System.Boolean, Guid, Guid&gt;.</returns>
        private (bool IsValid, Guid ApiKey, Guid UserKey) GetKeysFromFormData(NameValueCollection formData)
        {
            bool isValid = true;
            var apiKey = Guid.Empty;
            var userKey = Guid.Empty;
            var dic = formData.AllKeys
               .ToDictionary(t => t.ToLower(), t => formData[t]);

            if (!(dic.ContainsKey("apikey") & dic.ContainsKey("userkey")))
            {
                isValid = false;
            }
            else
            {
                if (!(Guid.TryParse(dic["apikey"], out apiKey) & Guid.TryParse(dic["userkey"], out userKey)))
                {
                    isValid = false;
                }
            }

            return (isValid, apiKey, userKey);

        }

        /// <summary>
        /// Gets the content of the keys from request.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="paraType">Type of the para.</param>
        /// <returns>System.ValueTuple&lt;System.Boolean, Guid, Guid&gt;.</returns>
        private (bool IsValid, Guid ApiKey, Guid UserKey) GetKeysFromRequestContent(string content, Type paraType)
        {
            bool isValid = true;
            var apiKey = Guid.Empty;
            var userKey = Guid.Empty;
            try
            {
                var appKey = JsonConvert.DeserializeObject(content, paraType) as IMobileAppSecurityKey;
                if (appKey == null)
                    isValid = false;
                else
                {
                    apiKey = appKey.ApiKey;
                    userKey = appKey.UserKey;
                }
            }
            catch
            {
                isValid = false;
            }
            return (isValid, apiKey, userKey);
        }


        /// <summary>
        /// Called when authorization is triggered pre controller method execution.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!this.AuthorizeCore(actionContext))
            {
                this.HandleUnauthorizedRequest(actionContext);
            }

        }

    }
}
