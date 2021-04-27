using Blazored.LocalStorage;
using Chilite.Protos;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace Chilite.Web
{
    public class AuthorizeApi
    {
        private readonly ILocalStorageService _localStorage;
        private readonly Task<Account.AccountClient> _accountClient;
        private readonly IdentityAuthenticationStateProvider _authenticationStateProvider;

        public AuthorizeApi(AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage, Task<Account.AccountClient> accountClient)
        {
            _localStorage = localStorage;
            _accountClient = accountClient;
            _authenticationStateProvider = (IdentityAuthenticationStateProvider) authenticationStateProvider;
        }

        public async Task<bool> Login(string login, string password)
        {
            try
            {
                var tokenResponse = await (await _accountClient).LoginAsync(new LoginRequest()
                {
                    Login = login,
                    Password = password
                });

                if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                {
                    var token = tokenResponse.Login.Token;
                    Console.WriteLine(token);
                    await _localStorage.SetItemAsync("token", token);
                    _authenticationStateProvider.MarkUserAsAuthenticated(token);

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public async Task<bool> Register(string username, string password)
        {
            try
            {
                var tokenResponse = await (await _accountClient).RegisterAsync(new RegisterRequest
                {
                    Login = username,
                    Password = password
                });

                if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                {
                    var token = tokenResponse.Login.Token;

                    await _localStorage.SetItemAsync("token", token);
                    _authenticationStateProvider.MarkUserAsAuthenticated(token);

                    return true;
                }
            }
            catch (Exception e)
            {
            }

            return false;
        }
    }
}