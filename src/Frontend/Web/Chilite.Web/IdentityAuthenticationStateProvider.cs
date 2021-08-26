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
        #region Private Fields

        private readonly ILocalStorageService _localStorage;
        private readonly Account.AccountClient _accountClient;

        #endregion

        #region Constructor

        public IdentityAuthenticationStateProvider(ILocalStorageService localStorage,
            Account.AccountClient accountClient)
        {
            _localStorage = localStorage;
            _accountClient = accountClient;
        }

        #endregion

        #region Public Methods

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("token");

            return await IsTokenValid(token)
                ? Jwt.GetStateFromJwt(token)
                : Empty();
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

        #endregion

        #region Private Fields

        private async Task<bool> IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var authUser =
                    await _accountClient.GetUserProfileAsync(new UserInfoRequest());

                if (authUser.ResultCase == UserInfoResponse.ResultOneofCase.Profile)
                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        private static AuthenticationState Empty()
            => new(new ClaimsPrincipal(new ClaimsIdentity()));

        #endregion
    }
}