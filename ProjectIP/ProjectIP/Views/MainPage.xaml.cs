
using ProjectIP.ViewModels;

namespace ProjectIP.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (MainPageViewModel) this.BindingContext;
            if (vm.IsFilteredWordsVisible)
            {
                vm.CloseFilteredWordsCommand.Execute();
                return true;
            }
            return false;
        }
    }
}
