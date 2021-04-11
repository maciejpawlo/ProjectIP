
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
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
using Xamarin.Forms;

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

        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        public MainPageViewModel(INavigationService navigationService, ITextToSpeech textToSpeechService, IAuthenticationService authenticationService)
            : base(navigationService)
        {
            Title = "Strona główna";
            _textToSpeechService = textToSpeechService;
            _authenticationService = authenticationService;
            TestTTSCommand = new DelegateCommand(TestTTS);
            SignOutCommand = new DelegateCommand(async () => await SignOut());
            AddWordCommand = new DelegateCommand(async () => await AddWord());
        }

        private async Task SignOut()
        {
            _authenticationService.SignOut();
            await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
        }

        private async void TestTTS()
        {
            var storage = new FirebaseStorage("projekt-ip.appspot.com",
                new FirebaseStorageOptions { AuthTokenAsyncFactory = async () => await _authenticationService.GetToken() });
            var url = await storage.Child("users").Child(_authenticationService.GetUid()).Child("artworks-000644051680-9dpi8s-t500x500.jpg").GetDownloadUrlAsync();
            Image = ImageSource.FromUri(new Uri(url));
            await _textToSpeechService.SpeakAsync("Hello World");
        }

        private async Task AddWord()
        {
            await NavigationService.NavigateAsync("NavigationPage/AddWordPage", null, true, true);
        }

    }
}
