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
        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand EmailTextChangedCommand { get; set; }
        public DelegateCommand PasswordTextChangedCommand { get; set; }
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
            EmailTextChangedCommand = new DelegateCommand(OnEmailTextChanged);
            PasswordTextChangedCommand = new DelegateCommand(OnPasswordTextChanged);
        }

        private async Task Register()
        {
            //TODO walidacja
            string token = await _authenticationService.RegisterWithEmailAndPassword(Email, Password);

            if (string.IsNullOrEmpty(token))
            {
                await _dialogService.DisplayAlertAsync("Coś poszło nie tak :(", "Podany email lub hasło są nieprawidłowe.", "OK");
                return;
            }
            //navigate to login page
           // await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
            await NavigationService.GoBackAsync();
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

        private async Task GoBack()
        {
            await NavigationService.GoBackAsync();
        }

    }
}
