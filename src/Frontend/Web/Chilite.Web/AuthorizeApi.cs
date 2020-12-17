using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chilite.Web
{
    public class AuthorizeApi
    {
        private readonly ILocalStorageService _localStorage;
        private IdentityAuthenticationStateProvider _authenticationStateProvider;

        public AuthorizeApi(AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _authenticationStateProvider = (IdentityAuthenticationStateProvider) authenticationStateProvider;
        }

        public async Task<bool> Login(string login, string password)
        {
            var token = "dfdfdferwrr";


            await _localStorage.SetItemAsync("token", token);

            _authenticationStateProvider.MarkUserAsAuthenticated(token);
            
            return true;
        }
    }
}