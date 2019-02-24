using Trident.Security;
using Trident.Web.Contracts;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Trident.Contracts;

namespace Trident.Web
{
    /// <summary>
    /// Class OwinSignInManager.
    /// Implements the <see cref="Trident.Security.ISignInProvider" />
    /// </summary>
    /// <seealso cref="Trident.Security.ISignInProvider" />
    public class OwinSignInManager : ISignInProvider
    {
        /// <summary>
        /// The owin context resolver
        /// </summary>
        private readonly IOwinContextResolver _owinContextResolver;
        /// <summary>
        /// The session manager
        /// </summary>
        private readonly ISessionManager _sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwinSignInManager"/> class.
        /// </summary>
        /// <param name="owinContextResolver">The owin context resolver.</param>
        /// <param name="sessionManager">The session manager.</param>
        public OwinSignInManager(IOwinContextResolver owinContextResolver, ISessionManager sessionManager)
        {
            _owinContextResolver = owinContextResolver;
            _sessionManager = sessionManager;
        }

        /// <summary>
        /// Logons the specified identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <exception cref="System.ArgumentNullException">Identity required</exception>
        public void Logon(params IIdentity[] identity)
        {
            var claimsIdentities = identity?.Cast<ClaimsIdentity>().ToArray();

            if (claimsIdentities == null || claimsIdentities.Length < 0)
            {
                throw new ArgumentNullException("Identity required");
            }

            var ctx = _owinContextResolver.GetContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(claimsIdentities);
            var prince = new ClaimsPrincipal(claimsIdentities);
            System.Web.HttpContext.Current.User = prince;
            System.Threading.Thread.CurrentPrincipal = prince;
            ctx.Request.User = prince;
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        public void Logout()
        {
            var ctx = _owinContextResolver.GetContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            _sessionManager.ClearAll();
            var prince =  new GenericPrincipal(new GenericIdentity(string.Empty), new string[] { });
            System.Web.HttpContext.Current.User = prince;
            System.Threading.Thread.CurrentPrincipal = prince;
            ctx.Request.User = prince;
        }
    }
}
