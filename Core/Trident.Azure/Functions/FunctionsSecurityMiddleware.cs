using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Trident.Azure.Security;
using Trident.IoC;
using Trident.Logging;
using Trident.Security;

namespace Trident.Azure.Functions
{
    public class FunctionsSecurityMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILog _logger;
        private readonly IFunctionControllerFactory _controllerFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIoCProvider _iocProvider;

        public FunctionsSecurityMiddleware(
            ILog appLoger,
            IFunctionControllerFactory controllerFactory,
            IAuthorizationService authorizationService,
            IIoCProvider iocProvider
            )
        {

            _logger = appLoger;
            _controllerFactory = controllerFactory;
            _authorizationService = authorizationService;
            _iocProvider = iocProvider;
        }


        private class Headers
        {
            public string Authorization { get; set; }
        }


        protected virtual Task<bool> IsAuthorizedPrecheck(JwtSecurityToken token, ClaimsPrincipal principal)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> IsAuthorizedPostcheck(JwtSecurityToken token, ClaimsPrincipal principal)
        {
            return Task.FromResult(true);
        }


        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                var log = context.GetLogger<FunctionsSecurityMiddleware>();
                var funcClassType = GetFunctionClassType(context.FunctionDefinition);

                if (funcClassType != null)
                {
                    var classLevelClaimsAuthAttrs = funcClassType.GetCustomAttributes<ClaimsAuthorizeAttribute>();
                    var methodLevelClaimsAuthAttrs = funcClassType.GetMethod(context.FunctionDefinition.Name)?.GetCustomAttributes<ClaimsAuthorizeAttribute>();

                    var claims = new List<ClaimsAuthorizeAttribute.ClaimEntity>(classLevelClaimsAuthAttrs.Select(x => x.Claim));
                    claims.AddRange(methodLevelClaimsAuthAttrs.Select(x => x.Claim));
                    var authorized = true;

                    if (claims.Any())
                    {
                        var headers = JsonConvert.DeserializeObject<Headers>(context.BindingContext.BindingData[nameof(Headers)].ToString());
                        var token = headers.Authorization.Replace("bearer", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
                        var principal = await _authorizationService.ValidateToken(token);
                        var securityToken = ReadJwtToken(token);

                        authorized &= await IsAuthorizedPrecheck(securityToken, principal);

                        foreach (var claim in claims)
                        {
                            //if (string.IsNullOrWhiteSpace(userClaimValue) || !userClaimValue.Contains(claim.Value))
                            if (!_authorizationService.HasPermission(principal, claim.Type, claim.Value))
                            {
                                authorized = false;
                                context.OverwriteResponseStream("false", System.Net.HttpStatusCode.Unauthorized);
                                _logger.Information<FunctionsSecurityMiddleware>(messageTemplate: $"{context.FunctionDefinition.EntryPoint}| User: {securityToken.Subject} doesn't have authorization claims for {claim.Type} with value {claim.Value}");
                                break;
                            }
                        }

                        authorized &= await IsAuthorizedPostcheck(securityToken, principal);
                    }


                    if (authorized)
                        await next(context);

                }
                else
                {
                    _logger.Information(messageTemplate: $"{context.FunctionDefinition.EntryPoint} is not registered for security evaluation. Doesn't implement IFunctionController interface.");

                    await next(context);

                }
            }
            catch (Exception ex)
            {
                _logger.Error<FunctionsSecurityMiddleware>(ex, $"{context.FunctionDefinition.EntryPoint} threw an exception.");
            }
        }

        private JwtSecurityToken ReadJwtToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler(); ;

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken;
        }

        private Type GetFunctionClassType(FunctionDefinition definition)
        {
            var pos = definition.EntryPoint.LastIndexOf('.');
            var fqcn = definition.EntryPoint.Substring(0, pos);
            return _controllerFactory.GetControllerType(fqcn);
        }
    }
}
