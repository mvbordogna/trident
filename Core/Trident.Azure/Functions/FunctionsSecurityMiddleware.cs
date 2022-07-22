using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Trident.Azure.Security;
using Trident.Contracts.Configuration;
using Trident.IoC;
using Trident.Logging;
using Trident.Security;

namespace Trident.Azure.Functions
{
    public class FunctionsSecurityMiddleware : IFunctionsWorkerMiddleware
    {
        protected readonly ILog Logger;
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

            Logger = appLoger;
            _controllerFactory = controllerFactory;
            _authorizationService = authorizationService;
            _iocProvider = iocProvider;
        }


        private class Headers
        {
            public string Authorization { get; set; }
        }


        protected virtual Task<bool> IsAuthorizedPrecheck(FunctionContext context, JwtSecurityToken token, ClaimsPrincipal principal)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> IsAuthorizedPostcheck(FunctionContext context, JwtSecurityToken token, ClaimsPrincipal principal)
        {
            return Task.FromResult(true);
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            bool enableSecurityExceptions = false;
            var settings = context.InstanceServices.GetService(typeof(IAppSettings)) as IAppSettings;
            bool.TryParse(settings["AppSettings:EnableSecurityExceptions"], out enableSecurityExceptions);

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


                        var principal =  await _authorizationService.ValidateToken(token);

                        if (principal != null)
                        {
                            var securityToken = ReadJwtToken(token);

                            authorized &= await IsAuthorizedPrecheck(context, securityToken, principal);

                            foreach (var claim in claims)
                            {
                                //if (string.IsNullOrWhiteSpace(userClaimValue) || !userClaimValue.Contains(claim.Value))
                                if (!_authorizationService.HasPermission(principal, claim.Type, claim.Value))
                                {
                                    authorized = false;
                                    var secMsg = $"{context.FunctionDefinition.EntryPoint} | User: {securityToken.Subject} doesn't have authorization claims for {claim.Type} with value {claim.Value}";
                                    var msg = (enableSecurityExceptions)
                                        ?  secMsg
                                        : "User is unauthorized.";
                                    
                                    context.OverwriteResponseStream(msg, System.Net.HttpStatusCode.Unauthorized);
                                    Logger.Information<FunctionsSecurityMiddleware>(messageTemplate: secMsg);
                                    break;
                                }
                            }

                            authorized &= await IsAuthorizedPostcheck(context, securityToken, principal);
                        }
                        else
                        {
                            authorized = false;
                            context.OverwriteResponseStream("Unauthorized Access", System.Net.HttpStatusCode.Unauthorized);
                            Logger.Information<FunctionsSecurityMiddleware>(messageTemplate: "Token deserialization didn't result into a Claims Principal.");
                        }
                    }

                    if (authorized)
                        await next(context);
                }
                else
                {
                    Logger.Information(messageTemplate: $"{context.FunctionDefinition.EntryPoint} is not registered for security evaluation. Doesn't implement IFunctionController interface.");
                    await next(context);

                }
            }
            catch (SecurityTokenException stv)
            {
                var msg = (enableSecurityExceptions)
                    ? stv.ToString()
                    : $"Unauthorized Access";

                context.OverwriteResponseStream(msg, System.Net.HttpStatusCode.Unauthorized);
                Logger.Error<FunctionsSecurityMiddleware>(stv, $"{context.FunctionDefinition.EntryPoint} Access Denied.");
            }
            catch (Exception ex)
            {   
                context.OverwriteResponseStream("Internal Server Error", System.Net.HttpStatusCode.InternalServerError);
                Logger.Error<FunctionsSecurityMiddleware>(ex, $"{context.FunctionDefinition.EntryPoint} threw an exception.");
             
            }
        }

        private JwtSecurityToken ReadJwtToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
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
