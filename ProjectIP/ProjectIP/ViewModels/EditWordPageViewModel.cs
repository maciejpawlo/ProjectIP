using Acr.UserDialogs;
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
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ProjectIP.ViewModels
{
    public class EditWordPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand EditWordCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand ShowPickerCommand { get; set; }
        public DelegateCommand SaveWordCommmand { get; set; }
        public DelegateCommand ValidateFormCommand { get; set; }
        #endregion

        #region Props
        private Word _wordToEdit;
        public Word WordToEdit
        {
            get { return _wordToEdit; }
            set { SetProperty(ref _wordToEdit, value); }
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
        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }
        private byte[] _imageBytes;
        public byte[] ImageBytes
        {
            get { return _imageBytes; }
            set { SetProperty(ref _imageBytes, value); }
        }
        private Stream _imageStream;
        public Stream ImageStream
        {
            get { return _imageStream; }
            set { SetProperty(ref _imageStream, value); }
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
        #endregion

        #region Services 
        public IDatabaseService _databaseService { get; private set; }
        public IStorageService _storageService { get; private set; }
        public IMediaPicker _mediaPickerService { get; private set; }
        public IUserDialogs _userDialogsService { get; private set; }
        public IAuthenticationService _authenticationService { get; private set; }
        #endregion

        public EditWordPageViewModel(INavigationService navigationService, IDatabaseService databaseService, IStorageService storageService,
            IMediaPicker mediaPicker, IUserDialogs userDialogs, IAuthenticationService authenticationService) : base(navigationService)
        {
            _databaseService = databaseService;
            _storageService = storageService;
            _mediaPickerService = mediaPicker;
            _userDialogsService = userDialogs;
            _authenticationService = authenticationService;

            GoBackCommand = new DelegateCommand(async () => await GoBack());
            ShowPickerCommand = new DelegateCommand(async () => await OpenFilePickerAsync());
            SaveWordCommmand = new DelegateCommand(async () => await SaveWord());
            ValidateFormCommand = new DelegateCommand(ValidateForm);

            Title = "Edytuj słowo";
        }

        private async Task GoBack()
        {
            await NavigationService.GoBackAsync();
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
            var uid = _authenticationService.GetUid();
            _userDialogsService.ShowLoading("Trwa zapis słowa...");
            if (!string.IsNullOrEmpty(FileName))
            {
                //TODO usuwanie starego zdjecia
                await _storageService.AddFile(ImageBytes, FileName);
                var imageUrl = await _storageService.GetFileUrl(FileName);
                WordToEdit.ImagePath = $"users/{uid}/{FileName}";
                WordToEdit.ImageUrl = imageUrl;
            }
            WordToEdit.Category = Category;
            WordToEdit.Description = Description;

            await _databaseService.EditWord(WordToEdit);
            _userDialogsService.HideLoading();
            await NavigationService.GoBackAsync();
        }

        public override void Initialize(INavigationParameters parameters)
        {
            WordToEdit = parameters.GetValue<Word>("modelToEdit");
            Description = WordToEdit.Description;
            Category = WordToEdit.Category;
            Image = ImageSource.FromUri(new Uri(WordToEdit.ImageUrl));
        }
        private void ValidateForm()
        {
            IsFormValid = IsDescValid;
        }
    }
}
