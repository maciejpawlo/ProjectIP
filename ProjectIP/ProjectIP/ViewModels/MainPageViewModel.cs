
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
        public DelegateCommand<string> FilterWordsCommand { get; set; }
        public DelegateCommand CloseFilteredWordsCommand { get; set; }

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

        private List<Word> _words;
        public List<Word> AllWords
        {
            get { return _words; }
            set { SetProperty(ref _words, value); }
        }

        private List<Word> _filteredWords;
        public List<Word> FilteredWords
        {
            get { return _filteredWords; }
            set { SetProperty(ref _filteredWords, value); }
        }

        private List<Word> _sentenceToRead;
        public List<Word> SentenceToRead
        {
            get { return _sentenceToRead; }
            set { SetProperty(ref _sentenceToRead, value); }
        }

        private bool _isCatListVisible;
        public bool IsCatListVisible
        {
            get { return _isCatListVisible; }
            set { SetProperty(ref _isCatListVisible, value); }
        }
        //Visibility of filtered words list
        private bool _isFilteredWordsVisible;
        public bool IsFilteredWordsVisible
        {
            get { return _isFilteredWordsVisible; }
            set { SetProperty(ref _isFilteredWordsVisible, value); }
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
            FilterWordsCommand = new DelegateCommand<string>((cat)=>GetFilteredWords(cat));
            CloseFilteredWordsCommand = new DelegateCommand(CloseFilteredWords);
            IsCatListVisible = true;
        }

        private async Task SignOut()
        {
            _authenticationService.SignOut();
            await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
        }

        private async Task TestTTS()
        {
           // var url = await _storage.Child("users").Child(Uid).Child("0AE852EB-76F6-4607-84AF-8845B1779A9A.jpeg").GetDownloadUrlAsync();
           // Image = ImageSource.FromUri(new Uri(url));
            await _textToSpeechService.SpeakAsync("Test test test");
        }

        private async Task AddWord()
        {
            await NavigationService.NavigateAsync("NavigationPage/AddWordPage", null, true, true);
        }

        private void GetFilteredWords(string category)
        {
            FilteredWords = AllWords.Where(x => x.Category == category).ToList();
            IsCatListVisible = false;
            IsFilteredWordsVisible = true;
            //TODO show filtered words list, hide categories list
        }

        private void CloseFilteredWords()
        {
            IsCatListVisible = !IsCatListVisible;
            IsFilteredWordsVisible = !IsFilteredWordsVisible;
            FilteredWords.Clear();
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Uid = _authenticationService.GetUid();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            AllWords = await _databaseService.GetAllWords();
        }

    }
}
