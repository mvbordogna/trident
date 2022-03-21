using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Trident.Common;

namespace Trident.Azure.Functions
{
    public class FunctionsSecurityMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly IAppSettings Settings;
        private readonly dynamic _logger;
        private readonly IFunctionControllerFactory _controllerFactory;

        public FunctionsSecurityMiddleware(
            IAppSettings settings,
            dynamic appLoger,
            IFunctionControllerFactory controllerFactory)
        {
            this.Settings = settings;
            _logger = appLoger;
            _controllerFactory = controllerFactory;
        }


        private class Headers
        {
            public string Authorization { get; set; }
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
                        var principal = ReadJwtToken(token);

                        foreach (var claim in claims)
                        {
                            var userClaimValue = principal.Claims.GetClaimValue(claim.Type);
                            if (string.IsNullOrWhiteSpace(userClaimValue) || !userClaimValue.Contains(claim.Value))
                            {
                                authorized = false;
                                context.OverwriteResponseStream("false", System.Net.HttpStatusCode.Unauthorized);
                                _logger.LogInformation($"{context.FunctionDefinition.EntryPoint}| User: {principal.Subject} doesn't have authorization claims for {claim.Type} with value {claim.Value}");
                                break;
                            }
                        }
                    }

                    if (authorized)
                        await next(context);

                }
                else
                {
                    _logger.LogInformation($"{context.FunctionDefinition.EntryPoint} is not registered for security evaluation. Doesn't implement IFunctionController interface.");
                    await next(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{context.FunctionDefinition.EntryPoint} threw an exception.");
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
