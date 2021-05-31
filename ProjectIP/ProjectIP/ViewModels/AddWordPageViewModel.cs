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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Acr.UserDialogs;

namespace ProjectIP.ViewModels
{
    public class AddWordPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand ShowPickerCommand { get; set; }
        public DelegateCommand SaveWordCommmand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand ValidateFormCommand { get; set; }
        #endregion

        #region Props
        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private Stream _imageStream;
        public Stream ImageStream
        {
            get { return _imageStream; }
            set { SetProperty(ref _imageStream, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
        private byte[] _imageBytes;
        public byte[] ImageBytes
        {
            get { return _imageBytes; }
            set { SetProperty(ref _imageBytes, value); }
        }

        private bool _isFormValid;
        public bool IsFormValid
        {
            get { return _isFormValid; }
            set { SetProperty(ref _isFormValid, value); }
        }
        private bool _isDescValid;
        public bool IsDescValid
        {
            get { return _isDescValid; }
            set { SetProperty(ref _isDescValid, value); }
        }
        private bool _isPickerValid;
        public bool IsPickerValid
        {
            get { return _isPickerValid; }
            set { SetProperty(ref _isPickerValid, value);}
        }

        #endregion

        #region Services
        public IAuthenticationService _authenticationService { get; private set; }
        public IDatabaseService _databaseService { get; private set; }
        public IStorageService _storageService { get; private set; }
        public IUserDialogs _userDialogsService { get; private set; }
        public IMediaPicker _mediaPickerService { get; private set; }
        #endregion

        public AddWordPageViewModel(INavigationService navigationService, IAuthenticationService authenticationService, 
           IUserDialogs userDialogsService, IDatabaseService databaseService, IMediaPicker mediaPicker, IStorageService storageService) : base (navigationService)
        {
            _userDialogsService = userDialogsService;
            _authenticationService = authenticationService;
            _mediaPickerService = mediaPicker;
            _databaseService = databaseService;
            _storageService = storageService;

            ShowPickerCommand = new DelegateCommand(async () => await OpenFilePickerAsync());
            SaveWordCommmand = new DelegateCommand(async () => await SaveWord());
            GoBackCommand = new DelegateCommand(async () => await GoBack());
            ValidateFormCommand = new DelegateCommand(ValidateForm);
            Title = "Dodaj nowe słowo";
        }

        private async Task OpenFilePickerAsync()
        {
            try
            {
                var result = await _mediaPickerService.PickPhotoAsync();
                if (result != null)
                {
                    FileName = result.FileName;
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        var fileByte = File.ReadAllBytes(result.FullPath);
                        ImageBytes = fileByte;
                        var stream = new MemoryStream(fileByte);
                        ImageStream = stream;
                        Image = ImageSource.FromStream(() => stream);
                        ValidateForm();
                    }
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
        }

        private async Task SaveWord()
        {
            //TODO w nazwie nie moze byc shared

            var uid = _authenticationService.GetUid();
            _userDialogsService.ShowLoading("Trwa zapis zdjęcia...");
            await _storageService.AddFile(ImageBytes, FileName);
            var imageUrl = await _storageService.GetFileUrl(FileName);
            var wordToSave = new Word()
            {
                ImageName = FileName,
                ImagePath = $"users/{uid}/{FileName}",
                Description = Description,
                Category = Category,
                ImageUrl = imageUrl
            };
            await _databaseService.AddWord(wordToSave);
            _userDialogsService.HideLoading();
            await NavigationService.GoBackAsync();
        }

        private async Task GoBack()
        {
            await NavigationService.GoBackAsync();
        }

        private void ValidateForm()
        {
            IsFormValid = IsDescValid && IsPickerValid && !string.IsNullOrEmpty(FileName);
        }
    }
}
