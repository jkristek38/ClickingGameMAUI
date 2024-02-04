using ClickingGame.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.ViewModels
{
    public partial class ClickerViewModel: ObservableObject, IRecipient<ProfileBoostExpiredMessage>, IRecipient<BoostBoughtMessage>, IRecipient<RefreshProfileMessage>, IRecipient<ProfileBoostRequestMessage>, IRecipient<ProfileCoinsRequestMessage>, IRecipient<ProfilePCoinsRequestMessage>, IRecipient<ProfileClickRequestMessage>
    {
        private readonly ProfileStore _profileStore;
        private Random _random;
        private int time_to_event;
        [ObservableProperty]
        private int coins;
        [ObservableProperty]
        private int premium_coins;
        [ObservableProperty]
        private Boost activeBoost;
        [ObservableProperty]
        private int clicks;
        [ObservableProperty]
        private Microsoft.Maui.Graphics.Color buttonBackground;
        [ObservableProperty]
        private Microsoft.Maui.Graphics.Color hasBoostButton;
        [ObservableProperty]
        private bool smile;
        [ObservableProperty]
        private bool boostNotActive;
        [ObservableProperty]
        private string boostText;
        public ClickerViewModel(ProfileStore profileStore)
        {
            BoostText = "buy boost";
            HasBoostButton = Colors.LightGray;
            BoostNotActive = false;
            Smile = false;
            ButtonBackground = Colors.Yellow;
            _random = new Random();
            time_to_event=_random.Next(2,10);
            _profileStore = profileStore;
            Clicks = profileStore.Clicks;
            WeakReferenceMessenger.Default.Register<ProfileBoostExpiredMessage>(this);
            WeakReferenceMessenger.Default.Register<BoostBoughtMessage>(this);
            WeakReferenceMessenger.Default.Register<RefreshProfileMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileClickRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileBoostRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileCoinsRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfilePCoinsRequestMessage>(this);
            RefreshProfile();
        }
        public void RefreshProfile()
        {
            WeakReferenceMessenger.Default.Send<ProfileClickRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileBoostRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileCoinsRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfilePCoinsRequestMessage>();
        }

        [RelayCommand]
        private void ClickerClicked()
        {
            ButtonBackground=Colors.Yellow;
            Smile = false;
            _profileStore.AddClick();
            int chance = _random.Next(0, 5);
            if (chance ==0)  _profileStore.AddPremiumCoins(false);
            else if(chance!=0) _profileStore.AddCoins(false);

            if (time_to_event == 0) {
                ButtonBackground = Colors.Green;
                Smile = true;
                if (_random.Next(0, 1) == 0) _profileStore.AddPremiumCoins(true);
                else _profileStore.AddCoins(true);
                time_to_event = _random.Next(2,Clicks);
            }
            time_to_event--;
        }
        [RelayCommand]
        private void ActivateBoost()
        {
            if (BoostNotActive)
            {
                _profileStore.Activate_Boost();
                RefreshProfile();
                if (ActiveBoost.IsActive) { BoostNotActive = false; HasBoostButton = Colors.Green; BoostText = "Boost activate"; }
                else { HasBoostButton = Colors.Red; BoostText = "Boost not active"; }
            }
        }
        public void Receive(ProfileBoostExpiredMessage message)
        {
            BoostText = "buy boost";
            BoostNotActive = false;
            HasBoostButton = Colors.LightGray;
        }

        public void Receive(BoostBoughtMessage message)
        {
            BoostNotActive = true;
            HasBoostButton = Colors.Blue;
            BoostText = "Boost bought";
        }
        public void Receive(RefreshProfileMessage message)
        {
            RefreshProfile();
        }

        public void Receive(ProfileBoostRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            ActiveBoost = message.Response;
            if (ActiveBoost != null)
            {
                BoostNotActive = true;
                HasBoostButton = Colors.Blue;
                BoostText = "Boost bought";
            }
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
    }
}
