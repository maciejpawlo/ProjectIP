
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
using Acr.UserDialogs;
using System.Collections.ObjectModel;

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
        public DelegateCommand<Word> TapTestCommand { get; set; }
        public DelegateCommand<Word> DeleteWordFromSentenceCommand { get; set; }
        public DelegateCommand<Word> DeleteWordCommand { get; set; }
        #endregion

        #region Services
        public ITextToSpeech _textToSpeechService { get; private set; }
        public IAuthenticationService _authenticationService { get; private set; }
        public IDatabaseService _databaseService { get; private set; }
        public IUserDialogs _userDialogsService { get; private set; }
        public IStorageService _storageService { get; private set; }
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

        private ObservableCollection<Word> _sentenceToRead;
        public ObservableCollection<Word> SentenceToRead
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

        private bool _isCatButtonEnabled;
        public bool IsCatButtonEnabled  
        {
            get { return _isCatButtonEnabled; }
            set { SetProperty(ref _isCatButtonEnabled, value); }
        }
        //Visibility of filtered words list
        private bool _isFilteredWordsVisible;
        public bool IsFilteredWordsVisible
        {
            get { return _isFilteredWordsVisible; }
            set { SetProperty(ref _isFilteredWordsVisible, value); }
        }

        private string _categoryFilter;
        public string CategoryFilter
        {
            get { return _categoryFilter; }
            set { SetProperty(ref _categoryFilter, value); }
        }
        #endregion
        public MainPageViewModel(INavigationService navigationService, ITextToSpeech textToSpeechService, 
            IAuthenticationService authenticationService, IDatabaseService databaseService, IUserDialogs userDialogsService,
            IStorageService storageService)
            : base(navigationService)
        {
            Title = "Strona główna";
            _textToSpeechService = textToSpeechService;
            _authenticationService = authenticationService;
            _databaseService = databaseService;
            _userDialogsService = userDialogsService;
            _storageService = storageService;

            TestTTSCommand = new DelegateCommand(async () => await TestTTS());
            SignOutCommand = new DelegateCommand(async () => await SignOut());
            AddWordCommand = new DelegateCommand(async () => await AddWord());
            FilterWordsCommand = new DelegateCommand<string>((cat)=>GetFilteredWords(cat));
            CloseFilteredWordsCommand = new DelegateCommand(CloseFilteredWords);
            DeleteWordFromSentenceCommand = new DelegateCommand<Word>((word) => DeleteWordFromSentence(word));
            TapTestCommand = new DelegateCommand<Word>((word) => TapTest(word));
            DeleteWordCommand = new DelegateCommand<Word>(async (word) => await DeleteWord(word));

            SentenceToRead = new ObservableCollection<Word>();
            FilteredWords = new List<Word>();
            IsCatListVisible = true;
        }

        private async Task SignOut()
        {
            _authenticationService.SignOut();
            await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
        }

        private async Task TestTTS()
        {
            var words = SentenceToRead?.Select(x => x.Description).ToList();
            var sentence = string.Join(" ", words);
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return;
            }
            await _textToSpeechService.SpeakAsync(sentence);
        }

        private async Task AddWord()
        {
            await NavigationService.NavigateAsync("NavigationPage/AddWordPage", null, true, true);
        }

        private async Task DeleteWord(Word word)
        {
            var resultDelete = await _userDialogsService.ConfirmAsync("Czy na pewno chcesz usunąć to słowo?", "Uwaga", "Tak", "Nie");
            if (!resultDelete)
            {
                return;
            }
            _userDialogsService.ShowLoading("Trwa usuwanie...");
            var result = await _storageService.DeleteFile(word.ImagePath);
            if (!result)
            {
                await _userDialogsService.AlertAsync("Nie można usunąć danego zdjęcia :(");
                _userDialogsService.HideLoading();
                return;
            }
            await _databaseService.DeleteWord(word);
            AllWords = await _databaseService.GetAllWords();
            GetFilteredWords(CategoryFilter);
            _userDialogsService.HideLoading();
        }

        private void GetFilteredWords(string category)
        {
            CategoryFilter = category;
            FilteredWords = AllWords.Where(x => x.Category == category).ToList();
            IsCatListVisible = false;
            IsFilteredWordsVisible = true;
        }

        private void CloseFilteredWords()
        {
            if (IsFilteredWordsVisible)
            {
                IsCatListVisible = !IsCatListVisible;
                IsFilteredWordsVisible = !IsFilteredWordsVisible;
                CategoryFilter = "";
                FilteredWords.Clear();
            }
        }

        private void DeleteWordFromSentence(Word word)
        {
            SentenceToRead.Remove(word);
            _userDialogsService.Toast($"Usunięto słowo \"{word.Description}\"", new TimeSpan(0, 0, 2));
        }

        private void TapTest(Word word)
        {
            SentenceToRead.Add(word);
            _userDialogsService.Toast($"Dodano słowo \"{word.Description}\"", new TimeSpan(0,0,2));
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Uid = _authenticationService.GetUid();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            IsCatButtonEnabled = false;
            AllWords = await _databaseService.GetAllWords();
            IsCatButtonEnabled = true;
        }

    }
}
