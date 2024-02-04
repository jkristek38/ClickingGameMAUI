using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.ViewModels
{
    [QueryProperty(nameof(ClickingGame.Models.Boost), nameof(ClickingGame.Models.Boost))]
    public partial class DetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ClickingGame.Models.Boost boost;

        [RelayCommand]
        Task Return() => Shell.Current.GoToAsync("..");
    }
}
