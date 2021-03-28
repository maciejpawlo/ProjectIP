using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIP.ViewModels
{
    public class AddWordPageViewModel : ViewModelBase
    {
        public AddWordPageViewModel(INavigationService navigationService) : base (navigationService)
        {
            Title = "Dodaj nowe słowo";
        }
    }
}
