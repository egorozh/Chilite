using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chilite.Web
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public IdentityAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("token");

            if (!string.IsNullOrEmpty(token))
            {
                var authUser = new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "admin")
                    }, "jwt"));


                return new AuthenticationState(authUser);
            }
            
            return new(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var authUser = new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin")
                }, "jwt"));

            var authState = Task.FromResult(new AuthenticationState(authUser));

            NotifyAuthenticationStateChanged(authState);
        }
    }
}