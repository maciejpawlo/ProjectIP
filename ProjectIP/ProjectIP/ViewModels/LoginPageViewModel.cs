using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ProjectIP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIP.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region props
        private string _login;
        public string Email
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand NavigateToRegisterCommand { get; set; }
        public IAuthenticationService _authenticationService { get; private set; }
        #endregion

        public LoginPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService) : base(navigationService)
        {
            _authenticationService = authenticationService;
            LoginCommand = new DelegateCommand(Login);
            Title = "Zaloguj się";
        }

        private async void Login()
        {
            string token = await _authenticationService.LoginWithEmailAndPassword(Email, Password);

  
            if (String.IsNullOrEmpty(token))
            {
                return;
            }
            //navigate to mainpage
            await NavigationService.NavigateAsync("app:///NavigationPage/MainPage");
        }

    }
}
