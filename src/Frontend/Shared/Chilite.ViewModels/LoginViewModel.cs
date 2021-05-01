using Chilite.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Prism.Events;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chilite.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public string Login { get; set; }

        public string Password { get; set; }

        public ICommand LoginCommand { get; }
            
        public LoginViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            LoginCommand = new AsyncCommand(OnLogin);
        }

        private async Task OnLogin()
        {
            if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
            {
                const string baseUri = "https://localhost:5001/";

                var acclient = GetAccountClient(baseUri);

                var tokenResponse = await acclient.LoginAsync(new LoginRequest()
                {
                    Login = Login,
                    Password = Password
                });

                if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                {
                    var token = tokenResponse.Login.Token;

                    Metadata headers = new();
                    if (!string.IsNullOrEmpty(token))
                        headers.Add("Authorization", $"Bearer {token}");

                    var res = await acclient.GetUserProfileAsync(new UserInfoRequest(), headers);
                    _eventAggregator.GetEvent<LoginEvent>().Publish();
                }
            }
        }

        private static Account.AccountClient GetAccountClient(string baseUri)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });

            return new Account.AccountClient(channel);
        }
    }

    internal class LoginEvent : PubSubEvent
    {
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly IErrorHandler _errorHandler;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            IErrorHandler errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }

        #endregion
    }

    public static class TaskUtilities
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}