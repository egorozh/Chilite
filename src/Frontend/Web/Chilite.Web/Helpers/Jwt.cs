using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chilite.Web
{
    internal static class Jwt
    {   
        public static AuthenticationState GetStateFromJwt(string token)
            => new(new ClaimsPrincipal(GetIdentityFromJwtToken(token)));

        private static ClaimsIdentity GetIdentityFromJwtToken(string token)
            => new(ParseClaimsFromJwt(token), "jwt");

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
            => new JwtSecurityTokenHandler().ReadJwtToken(jwt).Claims;
    }
}