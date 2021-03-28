
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ProjectIP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ProjectIP.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region Props
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
        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand NavigateToRegisterCommand { get; set; }
        #endregion

        #region Services
        public IAuthenticationService _authenticationService { get; private set; }
        public IPageDialogService _dialogService { get; private set; }
        #endregion

        public LoginPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService, IPageDialogService dialogService) : base(navigationService)
        {
            _authenticationService = authenticationService;
            _dialogService = dialogService;
            LoginCommand = new DelegateCommand(Login);
            Title = "Zaloguj się";
        }

        private async void Login()
        {
            //TODO walidacja
            string token = await _authenticationService.LoginWithEmailAndPassword(Email, Password);

            if (String.IsNullOrEmpty(token))
            {
                await _dialogService.DisplayAlertAsync("Coś poszło nie tak :(", "Podany email lub hasło są nieprawidłowe.", "OK");
                return;
            }
            //navigate to mainpage
            await NavigationService.NavigateAsync("app:///NavigationPage/MainPage");
        }

    }
}
