using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chilite.Backend.Auth
{
    public class ChatJwtValidator : ISecurityTokenValidator
    {
        #region Public Properties

        public bool CanValidateToken => true;
        public int MaximumTokenSizeInBytes { get; set; } = int.MaxValue;

        public string Audience { get; }
        public string Issuer { get; }
        public string SecretKey { get; }

        #endregion

        #region Constructor

        public ChatJwtValidator(TokenParameters tokenParameters)
        {
            Audience = tokenParameters.Audience;
            Issuer = tokenParameters.Issuer;
            SecretKey = tokenParameters.SecretKey;
        }

        #endregion

        #region Public Methods

        public bool CanReadToken(string securityToken) => true;

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = Issuer,
                ValidAudience = Audience,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
            };

            try
            {
                var claimsPrincipal =
                    handler.ValidateToken(securityToken, tokenValidationParameters, out validatedToken);

                return claimsPrincipal;
            }
            catch (Exception e)
            {
                validatedToken = new JwtSecurityToken();
                return new ClaimsPrincipal();
            }
        }

        #endregion
    }
}