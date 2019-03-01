using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Owin;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Trident.IoC;

//[assembly: OwinStartup("TestingConfiguration", typeof(StartupDemo.TestStartup))]

namespace Trident.Testing.TestScopes
{


    /// <summary>
    /// Class OwinControllerIntegrationTestScope.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OwinControllerIntegrationTestScope<T> : DisposableTestScope<T>
        where T : ApiController
    {
        /// <summary>
        /// The _server
        /// </summary>
        private TestServer _server;
        /// <summary>
        /// The _container
        /// </summary>
        private IContainer _container;
        /// <summary>
        /// The _HTTP configuration
        /// </summary>
        private HttpConfiguration _httpConfig;

        /// <summary>
        /// Gets the HTTP client.
        /// </summary>
        /// <value>The HTTP client.</value>
        public HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Gets the json formatter used .
        /// </summary>
        /// <value>The json formatter.</value>
        public JsonMediaTypeFormatter JsonFormatter { get; private set; }

        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// The key that is to get the user dictionary key for the user to be spoofed on a specific request.
        /// </summary>
        public const string USER_HEADER_KEY = "UserKey";

        /// <summary>
        /// The Default header value for the user when no UserKey request header is supplied.
        /// </summary>
        public const string USER_HEADER_DEFAULT_USER = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="OwinControllerIntegrationTestScope{T}"/> class.
        /// </summary>
        public OwinControllerIntegrationTestScope()
        {
            JsonFormatter = new JsonMediaTypeFormatter();
            _httpConfig = new HttpConfiguration();
            var builder = new ContainerBuilder();          
            builder.RegisterType<T>();
            InitializeScope(builder, _httpConfig);
            _container = builder.Build();
            var verifier = new VerifiableContainerProvider(_container);
            verifier.VerifyAndThrow();

            _server = TestServer.Create(StartupConfiguration);
            HttpClient = _server.HttpClient;

        }

        protected override void DisposeResource()
        {
            base.DisposeResource();
            HttpClient = null;
            _container.Dispose();
            _httpConfig.Dispose();
            _server.Dispose();
        }

        /// <summary>
        /// Initializes the scope.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="config">The configuration.</param>
        protected abstract void InitializeScope(ContainerBuilder container, HttpConfiguration config);

        /// <summary>
        /// Adds the claims to authorize the user for full permissions when the userKey value is Default.
        /// Any other user key, do not add permissions so we can verify failure for unauthorized.
        /// if the success cases start to fail after changes a new permission was added.
        /// if the unauthorized test cases start to fail, then the permission 
        /// attribute was removed from the controller method.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="userKey">The user key.</param>
        protected abstract void AddClaims(IOwinContext context, ClaimsIdentity identity, string userKey);

        /// <summary>
        /// Gets the user principal.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userKey">The user key.</param>
        /// <returns>ClaimsPrincipal.</returns>
        private ClaimsPrincipal GetUserPrincipal(IOwinContext context, string userKey)
        {
            var identity = new ClaimsIdentity("Bearer", "name", "role");
            UserId = Guid.NewGuid();
            identity.AddClaim(new Claim("sub", UserId.ToString()));
            AddClaims(context, identity, userKey);
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Gets the injection configuration.
        /// </summary>
        /// <param name="container">The simple injector container.</param>
        /// <returns>HttpConfiguration.</returns> 
        protected virtual HttpConfiguration GetInjectionConfiguration(IContainer container)
        {
            var config = _httpConfig;
            config.Services.Replace(typeof(IHttpControllerTypeResolver), new IntegrationTestHttpControllerTypeResolver(typeof(T)));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(_container);
            return config;
        }

        /// <summary>
        /// Startups the configuration.
        /// </summary>
        /// <param name="app">The application.</param>
        protected void StartupConfiguration(IAppBuilder app)
        {
            var config = GetInjectionConfiguration(this._container);
            config.MapHttpAttributeRoutes();
            RegisterUserResolverFilter(app);
            app.UseWebApi(config);
        }

        /// <summary>
        /// Registers the user resolver filter.
        /// </summary>
        /// <param name="app">The application.</param>
        private void RegisterUserResolverFilter(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var userKey = context.Request.Headers[USER_HEADER_KEY];

                context.Request.User = this.GetUserPrincipal(context, userKey ?? USER_HEADER_DEFAULT_USER);
                await next.Invoke();
            });

        }

        /// <summary>
        /// Class IntegrationTestHttpControllerTypeResolver.
        /// </summary>
        private class IntegrationTestHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
        {
            /// <summary>
            /// The _target type
            /// </summary>
            private static Type _targetType = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="IntegrationTestHttpControllerTypeResolver"/> class.
            /// </summary>
            /// <param name="targetType">Type of the target.</param>
            public IntegrationTestHttpControllerTypeResolver(Type targetType)
                    : base(IsHttpEndpoint)
            {
                _targetType = targetType;
            }

            /// <summary>
            /// Determines whether [is HTTP endpoint] [the specified t].
            /// </summary>
            /// <param name="t">The t.</param>
            /// <returns><c>true</c> if [is HTTP endpoint] [the specified t]; otherwise, <c>false</c>.</returns>
            /// <exception cref="System.ArgumentNullException">t</exception>
            internal static bool IsHttpEndpoint(Type t)
            {
                if (t == null) throw new ArgumentNullException("t");

                return
                 t.IsClass &&
                 t.IsVisible &&
                 !t.IsAbstract && t == _targetType;
            }
        }

        private class VerifiableContainerProvider : AutofacIoCProvider
        {
            public VerifiableContainerProvider(IContainer container):base(null)
            {
                this.Container = container;

            }
        }
    }

   

}

