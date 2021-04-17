using Prism;
using Prism.Ioc;
using Prism.Navigation;
using ProjectIP.Interfaces;
using ProjectIP.ViewModels;
using ProjectIP.Views;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Acr.UserDialogs;

namespace ProjectIP
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            //await NavigationService.NavigateAsync("NavigationPage/MainPage");
            var authentication = Container.Resolve<IAuthenticationService>();
            if (authentication.IsSignedIn())
            {
                await NavigationService.NavigateAsync("NavigationPage/MainPage");
            }
            else
            {
                //await NavigationService.NavigateAsync("LoginPage", null, true, true);
                await NavigationService.NavigateAsync("app:///NavigationPage/LoginPage");
            }
            
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterSingleton<ITextToSpeech, TextToSpeechImplementation>();
            containerRegistry.RegisterSingleton<IFilePicker, FilePickerImplementation>();
            containerRegistry.RegisterSingleton<IPermissions, PermissionsImplementation>();
            containerRegistry.RegisterInstance<IUserDialogs>(UserDialogs.Instance);

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<PrismContentPage, PrismContentPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<AddWordPage, AddWordPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
        }
    }
}
