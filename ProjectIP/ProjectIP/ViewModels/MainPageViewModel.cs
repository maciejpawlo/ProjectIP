using Firebase.Database;
using Firebase.Database.Query;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ProjectIP.Interfaces;
using ProjectIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials.Interfaces;

namespace ProjectIP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region props
        public ITextToSpeech TextToSpeechService { get; private set; }
        public IAuthenticationService _authenticationService { get; private set; }
        public DelegateCommand TestTTSCommand { get; set; }
        public DelegateCommand SignOutCommand { get; set; }
        #endregion
        public MainPageViewModel(INavigationService navigationService, ITextToSpeech textToSpeechService, IAuthenticationService authenticationService)
            : base(navigationService)
        {
            Title = "Main Page";
            TextToSpeechService = textToSpeechService;
            _authenticationService = authenticationService;
            TestTTSCommand = new DelegateCommand(TestTTS);
            SignOutCommand = new DelegateCommand(SignOut);
        }

        private async void SignOut()
        {
            _authenticationService.SignOut();
            // NavigationService.NavigateAsync("LoginPage", null, true, true);
            await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
        }

        private void TestTTS()
        {
            //TextToSpeechService.SpeakAsync("Hello World");
        }
    }
}
