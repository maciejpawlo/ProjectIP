using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ProjectIP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIP.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
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
            set { SetProperty(ref _password, value);  }
        }
        #endregion

        #region Commands
        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        #endregion GoBackCommand

        #region Services
        public IAuthenticationService _authenticationService { get; private set; }
        public IPageDialogService _dialogService { get; private set; }
        #endregion

        public RegisterPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService,
            IPageDialogService dialogService) : base(navigationService)
        {
            _authenticationService = authenticationService;
            _dialogService = dialogService;
            RegisterCommand = new DelegateCommand(async () => await Register());
            GoBackCommand = new DelegateCommand(async () => await GoBack());
        }

        private async Task Register()
        {
            //TODO walidacja
            string token = await _authenticationService.RegisterWithEmailAndPassword(Email, Password);

            if (String.IsNullOrEmpty(token))
            {
                await _dialogService.DisplayAlertAsync("Coś poszło nie tak :(", "Podany email lub hasło są nieprawidłowe.", "OK");
                return;
            }
            //navigate to login page
           // await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
            await NavigationService.GoBackAsync();
        }

        private async Task GoBack()
        {
            await NavigationService.GoBackAsync();
        }

    }
}
