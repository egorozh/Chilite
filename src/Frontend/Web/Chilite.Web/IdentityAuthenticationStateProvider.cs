using Blazored.LocalStorage;
using Chilite.Protos;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Chilite.Web
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly Task<Account.AccountClient> _accountClient;

        public IdentityAuthenticationStateProvider(ILocalStorageService localStorage,
            Task<Account.AccountClient> accountClient)
        {
            _localStorage = localStorage;
            _accountClient = accountClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var authUser =
                        await (await _accountClient).GetUserProfileAsync(new UserInfoRequest());

                    if (authUser.ResultCase == UserInfoResponse.ResultOneofCase.Profile)
                        return Jwt.GetStateFromJwt(token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return Empty();
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var authState = Task.FromResult(Jwt.GetStateFromJwt(token));

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkLogouted()
        {
            await _localStorage.RemoveItemAsync("token");
            NotifyAuthenticationStateChanged(Task.FromResult(Empty()));
        }

        private static AuthenticationState Empty()
            => new(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}