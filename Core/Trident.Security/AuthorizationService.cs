using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Trident.Contracts;
using Trident.Contracts.Configuration;
using Trident.Extensions;

namespace Trident.Security 
{
	public class AuthorizationService : IAuthorizationService, IService
	{
		private readonly ILogger<AuthorizationService> _logger;

		private static readonly object _lock = new object();

		private Dictionary<string, string> _userPermissions = null;

		private readonly IAppSettings _appSettings;

		public AuthorizationService(IAppSettings appSettings, ILogger<AuthorizationService> logger)
		{
			_appSettings = appSettings;
			_logger = logger;
		}

		public async Task<ClaimsPrincipal> ValidateToken(string token)
		{
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			TokenValidation tokenValidationConfig = _appSettings.GetSection<TokenValidation>("TokenValidation");

			var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
				tokenValidationConfig.ValidIssuer + "/.well-known/oauth-authorization-server",
				new OpenIdConnectConfigurationRetriever(),
				new HttpDocumentRetriever());


			var discoveryDocument = await configurationManager.GetConfigurationAsync(default(CancellationToken));
			var signingKeys = discoveryDocument.SigningKeys;

			try
			{
				var tokenValidationParmaeters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = tokenValidationConfig.ValidIssuer,
					ValidAudience = tokenValidationConfig.ValidAudience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenValidationConfig.IssuerSigningKey)),
					IssuerSigningKeys = signingKeys
				};			

				SecurityToken validatedToken;
				var principal = tokenHandler.ValidateToken(token, tokenValidationParmaeters, out validatedToken);
				return principal;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured while validating the users authorization token.");
				throw;
			}		
		
		}

		public string GetClaim(ClaimsPrincipal user, string name)
		{
			if (!IsUserAuthenticated(user))
			{
				return string.Empty;
			}
			return user?.Claims.FirstOrDefault((Claim x) => x.Type == name)?.Value ?? string.Empty;
		}

		public bool HasClaim(ClaimsPrincipal user, string name, string operations)
		{
			if (!IsUserAuthenticated(user))
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(operations))
			{
				throw new ArgumentException("'operations' cannot be null or whitespace.", "operations");
			}
			string claim = GetClaim(user, name);
			return claim == operations;
		}

		public bool HasPermission(ClaimsPrincipal user, string name, string operations)
		{
			try
			{
				if (!IsUserAuthenticated(user))
				{
					return false;
				}
				Dictionary<string, string> permissions = GetPermissionsFromJwt(user);
				permissions = new Dictionary<string, string>(permissions, StringComparer.OrdinalIgnoreCase);
				if (!permissions.TryGetValue(name, out var op))
				{
					return false;
				}
				return HasOperation(operations, op);
			}
			catch (Exception ex)
			{
				ILogger<AuthorizationService> logger = _logger;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Error checking for claim '");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(":");
				defaultInterpolatedStringHandler.AppendFormatted(operations);
				defaultInterpolatedStringHandler.AppendLiteral("': ");
				defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
				logger.LogError(ex, defaultInterpolatedStringHandler.ToStringAndClear());
				throw;
			}
		}

		private bool IsUserAuthenticated(ClaimsPrincipal user)
		{
			return user?.Identity.IsAuthenticated ?? false;
		}

		private static bool HasOperation(string source, string target)
		{
			return source.All((char s) => target.Contains(s, StringComparison.OrdinalIgnoreCase));
		}

		private Dictionary<string, string> GetPermissionsFromJwt(ClaimsPrincipal user)
		{
			if (user != null && user.Identity!.IsAuthenticated)
			{
				if (_userPermissions == null)
				{
					lock (_lock)
					{
						if (_userPermissions == null)
						{
							_userPermissions = GetPermissionsFromJwtImpl(user);
						}
					}
				}
			}
			else
			{
				_userPermissions = null;
			}
			return _userPermissions;
		}

		private Dictionary<string, string> GetPermissionsFromJwtImpl(ClaimsPrincipal user)
		{
			Claim permissionsClaim = user.FindFirst("permissions");
			if (permissionsClaim?.Value != null)
			{
				return DecodePermissions(permissionsClaim.Value);
			}
			return new Dictionary<string, string>();
		}

		private static Dictionary<string, string> DecodePermissions(string encoded)
		{
			string decoded = encoded.FromBase64();
			Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(decoded);
			return dict ?? new Dictionary<string, string>();
		}
	}
}