
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

        private string _uid;
        public string Uid
        {
            get { return _uid; }
            set { SetProperty(ref _uid, value); }
        }

        public MainPageViewModel(INavigationService navigationService, ITextToSpeech textToSpeechService, IAuthenticationService authenticationService)
            : base(navigationService)
        {
            Title = "Strona główna";
            _textToSpeechService = textToSpeechService;
            _authenticationService = authenticationService;
            TestTTSCommand = new DelegateCommand(async () => await TestTTS());
            SignOutCommand = new DelegateCommand(async () => await SignOut());
            AddWordCommand = new DelegateCommand(async () => await AddWord());
        }

        private async Task SignOut()
        {
            _authenticationService.SignOut();
            await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
        }

        private async Task TestTTS()
        {
            var uid = _authenticationService.GetUid();

            var storage = new FirebaseStorage("projekt-ip.appspot.com",
                new FirebaseStorageOptions { AuthTokenAsyncFactory = async () => await _authenticationService.GetToken() });
            var url = await storage.Child("users").Child(uid).Child("artworks-000644051680-9dpi8s-t500x500.jpg").GetDownloadUrlAsync();
            Image = ImageSource.FromUri(new Uri(url));

            //await _textToSpeechService.SpeakAsync("Hello World");
        }

        private async Task AddWord()
        {
            await NavigationService.NavigateAsync("NavigationPage/AddWordPage", null, true, true);
        }
        public override void Initialize(INavigationParameters parameters)
        {
            Uid = _authenticationService.GetUid(); //todo zmienic pozniej na dodawanie do app preferences
            //.Initialize(parameters);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            var firebaseClient = new FirebaseClient("https://projekt-ip-default-rtdb.europe-west1.firebasedatabase.app/",
               new FirebaseOptions { AuthTokenAsyncFactory = async () => await _authenticationService.GetToken() });
            var words = (await firebaseClient.Child("word").Child(Uid).OnceAsync<Word>()).Select(x => x.Object).ToList();
            //base.OnNavigatedTo(parameters);
        }

    }
}
