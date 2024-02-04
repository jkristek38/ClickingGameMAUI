using ClickingGame.Models;
using ClickingGame.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.ViewModels
{
    [QueryProperty(nameof(IsLoggedIn), nameof(IsLoggedIn))]
    public partial class ProfileViewModel : ObservableObject, IRecipient<UserLoggedInMessage>, IRecipient<RefreshProfileMessage>, IRecipient<ProfileBoostRequestMessage>, IRecipient<ProfileCoinsRequestMessage>, IRecipient<ProfilePCoinsRequestMessage>, IRecipient<ProfileClickRequestMessage>, IRecipient<ProfileNameRequestMessage>
    {
        [ObservableProperty]
        private bool isLoggedIn;
        [ObservableProperty]
        private bool loginInNeeded;

        [ObservableProperty]
        private int coins;
        [ObservableProperty]
        private int premium_coins;
        [ObservableProperty]
        private Boost activeBoost;
        [ObservableProperty]
        private int clicks;
        [ObservableProperty]
        private string name;

        public ProfileViewModel(ProfileStore profileStore)
        {
            Name = profileStore.Name;
            IsLoggedIn = false;
            LoginInNeeded = true;
            WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this);
            WeakReferenceMessenger.Default.Register<RefreshProfileMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileNameRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileClickRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileBoostRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileCoinsRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfilePCoinsRequestMessage>(this);
            RefreshProfile();
        }
        public void RefreshProfile()
        {
            WeakReferenceMessenger.Default.Send<ProfileNameRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileClickRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileBoostRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileCoinsRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfilePCoinsRequestMessage>();
        }

        public void Receive(UserLoggedInMessage message)
        {
            LoginInNeeded = false;
            IsLoggedIn = true;
        }

        [RelayCommand]
        private Task LogIn() => Shell.Current.GoToAsync($"{nameof(LoginView)}");
        [RelayCommand]
        private Task SignIn() => Shell.Current.GoToAsync($"{nameof(SigninView)}");
        public void Receive(RefreshProfileMessage message)
        {
            RefreshProfile();
        }

        public void Receive(ProfileBoostRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            ActiveBoost = message.Response;
        }

        public void Receive(ProfileCoinsRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            Coins = message.Response;
        }

        public void Receive(ProfilePCoinsRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            Premium_coins = message.Response;
        }

        public void Receive(ProfileClickRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            Clicks = message.Response;
        }
        public void Receive(ProfileNameRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            Name = message.Response;
        }
    }
}
