﻿
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ProjectIP.Interfaces;
using Prism.Ioc;
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
        public IDatabaseService _databaseService { get; private set; }
        public FirebaseStorage _storage { get; private set; }
        #endregion

        #region Props
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
        #endregion
        public MainPageViewModel(INavigationService navigationService, ITextToSpeech textToSpeechService, 
            IAuthenticationService authenticationService, IDatabaseService databaseService)
            : base(navigationService)
        {
            Title = "Strona główna";
            _textToSpeechService = textToSpeechService;
            _authenticationService = authenticationService;
            _databaseService = databaseService;
            _storage = App.Current.Container.Resolve<FirebaseStorage>();
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
            var url = await _storage.Child("users").Child(Uid).Child("0AE852EB-76F6-4607-84AF-8845B1779A9A.jpeg").GetDownloadUrlAsync();
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
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            var testServ = await _databaseService.GetAllWords();
        }

    }
}