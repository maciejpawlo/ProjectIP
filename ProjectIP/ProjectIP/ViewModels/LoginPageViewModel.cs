
using Acr.UserDialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ProjectIP.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private bool _isValidEmail;
        public bool IsValidEmail
        {
            get { return _isValidEmail; }
            set { SetProperty(ref _isValidEmail, value); }
        }
        private bool _isValidPassword;
        public bool IsValidPassword
        {
            get { return _isValidPassword; }
            set { SetProperty(ref _isValidPassword, value); }
        }
        private bool _isEmailErrorVisible;
        public bool IsEmailErrorVisible
        {
            get { return _isEmailErrorVisible; }
            set { SetProperty(ref _isEmailErrorVisible, value); }
        }
        private bool _isPasswordErrorVisible;
        public bool IsPasswordErrorVisible
        {
            get { return _isPasswordErrorVisible; }
            set { SetProperty(ref _isPasswordErrorVisible, value); }
        }
        private bool _isFormValid;
        public bool IsFormValid
        {
            get { return _isFormValid; }
            set { SetProperty(ref _isFormValid, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand NavigateToRegisterCommand { get; set; }
        public DelegateCommand EmailTextChangedCommand { get; set; }
        public DelegateCommand PasswordTextChangedCommand { get; set; }
        #endregion

        #region Services
        public IAuthenticationService _authenticationService { get; private set; }
        public IUserDialogs _userDialogsService { get; set; }
        #endregion

        public LoginPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService,
            IUserDialogs userDialogsService) : base(navigationService)
        {
            _authenticationService = authenticationService;
            _userDialogsService = userDialogsService;
            LoginCommand = new DelegateCommand(async () => await Login());
            NavigateToRegisterCommand = new DelegateCommand(async () => await NavigateToRegister());
            EmailTextChangedCommand = new DelegateCommand(OnEmailTextChanged);
            PasswordTextChangedCommand = new DelegateCommand(OnPasswordTextChanged);
            IsEmailErrorVisible = false;
            Title = "Zaloguj się";
        }

        private async Task Login()
        {
            //walidacja
            string token = null;
            if (IsFormValid)
            {
                 token = await _authenticationService.LoginWithEmailAndPassword(Email, Password);
            }

            if (string.IsNullOrEmpty(token))
            {
                await _userDialogsService.AlertAsync("Podany email lub hasło są nieprawidłowe.", "Coś poszło nie tak :(", "OK");
                return;
            }
            //navigate to mainpage
            _userDialogsService.ShowLoading("Ładowanie...");
            await NavigationService.NavigateAsync("app:///NavigationPage/MainPage");
            _userDialogsService.HideLoading();
        }

        private void OnEmailTextChanged() //tu zmieniana jest widocznosc bledow zwiazanych z walidacja emaila
        {
            IsEmailErrorVisible = !IsValidEmail;
            IsFormValid = IsValidEmail && IsValidPassword;
        }

        private void OnPasswordTextChanged() //tu zmieniana jest widocznosc bledow zwiazanych z walidacja hasla
        {
            IsPasswordErrorVisible = !IsValidPassword;
            IsFormValid = IsValidEmail && IsValidPassword;
        }

        private async Task NavigateToRegister()
        {
            await NavigationService.NavigateAsync("NavigationPage/RegisterPage", null, true, true);
        }

    }
}
