
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
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace ProjectIP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand TestTTSCommand { get; set; }
        public DelegateCommand SignOutCommand { get; set; }
        public DelegateCommand AddWordCommand { get; set; }
        #endregion

        #region Services
        public ITextToSpeech _textToSpeechService { get; private set; }
        public IAuthenticationService _authenticationService { get; private set; }
        #endregion

        public MainPageViewModel(INavigationService navigationService, ITextToSpeech textToSpeechService, IAuthenticationService authenticationService)
            : base(navigationService)
        {
            Title = "Strona główna";
            _textToSpeechService = textToSpeechService;
            _authenticationService = authenticationService;
            TestTTSCommand = new DelegateCommand(TestTTS);
            SignOutCommand = new DelegateCommand(SignOut);
            AddWordCommand = new DelegateCommand(async()=> await AddWord());
        }

        private async void SignOut()
        {
            _authenticationService.SignOut();
            // NavigationService.NavigateAsync("LoginPage", null, true, true);
            await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
        }

        private async void TestTTS()
        {
            var uid = _authenticationService.GetUid();
            var firebaseClient = new FirebaseClient("https://projekt-ip-default-rtdb.europe-west1.firebasedatabase.app/",
                new FirebaseOptions { AuthTokenAsyncFactory = () => _authenticationService.GetToken() });
            await firebaseClient.Child("word").Child(uid).PostAsync(new Word()
            {
                ImageName = "pies.png",
                ImagePath = $"users/{uid}/test.png",
                Description = "Pies"
            });
            //_textToSpeechService.SpeakAsync("Hello World");
        }

        private async Task AddWord()
        {
            await NavigationService.NavigateAsync("NavigationPage/AddWordPage", null, true, true);
        }

    }
}
