using Chilite.Backend.Auth;
using Chilite.Database;
using Chilite.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Chilite.DomainModels;

namespace Chilite.Backend
{
    [Authorize]
    public class AccountService : Account.AccountBase
    {
        #region Private Fields

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ChatUser> _userManager;
        private readonly TokenParameters _tokenParameters;

        #endregion

        #region Constructor

        public AccountService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ChatUser> userManager,
            TokenParameters tokenParameters)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _tokenParameters = tokenParameters;
        }

        #endregion

        #region Public Methods

        [AllowAnonymous]
        public override async Task<LoginResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Login))
                return ErrorResponse("Login is not valid");

            ChatUser user = new()
            {
                UserName = request.Login,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (result.Succeeded)
            {
                var userIdentity = await _userManager.FindByNameAsync(user.UserName);

                return TokenResponse(await userIdentity.GenerateJwtToken(_tokenParameters, _roleManager, _userManager));
            }

            return new()
            {
                Error = new Error
                {
                    Message = result.Errors.FirstOrDefault()?.Description
                }
            };
        }

        [AllowAnonymous]
        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var user = await _userManager.FindByNameAsync(request.Login);

            if (user == null)
                return ErrorResponse("User not found");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isValidPassword)
                return ErrorResponse("Password wrong");

            return TokenResponse(await user.GenerateJwtToken(_tokenParameters, _roleManager, _userManager));
        }
        
        public override async Task<UserInfoResponse> GetUserProfile(UserInfoRequest request, ServerCallContext context)
        {
            var user = await _userManager.GetUserAsync(context.GetHttpContext().User);

            if (user == null)
                return new UserInfoResponse()
                {
                    Error = new Error
                    {
                        Message = "No access"
                    }
                };

            return new UserInfoResponse
            {
                Profile = new UserProfileInfo
                {
                    Username = user.UserName
                }
            };
        }

        #endregion

        #region Private Methods

        private LoginResponse ErrorResponse(string errorMessage) =>
            new()
            {
                Error = new Error
                {
                    Message = errorMessage
                }
            };

        private LoginResponse TokenResponse(string token) =>
            new()
            {
                Login = new LoginInfo
                {
                    Token = token
                }
            };

        #endregion
    }
}