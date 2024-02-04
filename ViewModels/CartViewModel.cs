using ClickingGame.Models;
using ClickingGame.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.ViewModels
{
    public partial class CartViewModel : ObservableValidator, IRecipient<UserLoggedInMessage>, IRecipient<CartBoostAddedMessage>,IRecipient<CartCoinAddedMessage>, IRecipient<CartPCoinAddedMessage>, IRecipient<CartBoostRequestMessage>, IRecipient<CartCoinsRequestMessage>, IRecipient<CartPCoinsRequestMessage>,IRecipient<CartCoinRemovedMessage>, IRecipient<CartPCoinRemovedMessage>, IRecipient<RefreshProfileMessage>, IRecipient<ProfileBoostRequestMessage>, IRecipient<ProfileCoinsRequestMessage>, IRecipient<ProfilePCoinsRequestMessage>
    {
        private readonly ProfileStore _profileStore;
        [ObservableProperty]
        private int coins;
        [ObservableProperty]
        private int premium_coins;
        [ObservableProperty]
        private Boost activeBoost;

        [ObservableProperty]
        private Microsoft.Maui.Graphics.Color hasBoostButton;
        [ObservableProperty]
        private bool boostNotActive;
        [ObservableProperty]
        private string boostText;
        private readonly ObservableCollection<Shop_Coins> _coins;
        public ObservableCollection<Shop_Coins> NormalCoins => _coins;
        private readonly ObservableCollection<Shop_Coins> _pcoins;
        public ObservableCollection<Shop_Coins> PCoins => _pcoins;

        private readonly ObservableCollection<Boost> _boosts;
        public ObservableCollection<Boost> Boosts => _boosts;

        [ObservableProperty]
        [IsTrue]
        private bool _hasAgreedToTermsAndConditions;
        [ObservableProperty]
        private string _hasAgreedToTermsAndConditionsText = "Agree to terms and conditions";
        [ObservableProperty]
        [IsTrue]
        private bool isLoggedIn;
        [ObservableProperty]
        private bool loadingIsBusy;
        [ObservableProperty]
        private string isLoggedInText;

        public CartViewModel(ProfileStore profileStore)
        {
            BoostText = "buy boost";
            _profileStore = profileStore;
            HasBoostButton = Colors.LightGray;
            LoadingIsBusy = true;
            _coins = new ObservableCollection<Shop_Coins>();
            _pcoins = new ObservableCollection<Shop_Coins>();
            _boosts = new ObservableCollection<Boost>();
            WeakReferenceMessenger.Default.Register<CartBoostAddedMessage>(this/*, (r, m) => { AddCartItem(m.Game); }*/);
            WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this);

            WeakReferenceMessenger.Default.Register<CartCoinAddedMessage>(this/*, (r, m) => { AddCartItem(m.Game); }*/);
            WeakReferenceMessenger.Default.Register<CartCoinRemovedMessage>(this/*, (r, m) => { AddCartItem(m.Game); }*/);           

            WeakReferenceMessenger.Default.Register<CartPCoinAddedMessage>(this/*, (r, m) => { AddCartItem(m.Game); }*/);
            WeakReferenceMessenger.Default.Register<CartPCoinRemovedMessage>(this/*, (r, m) => { AddCartItem(m.Game); }*/);

            WeakReferenceMessenger.Default.Register<RefreshProfileMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileBoostRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileCoinsRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfilePCoinsRequestMessage>(this);

            WeakReferenceMessenger.Default.Register<CartBoostRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<CartCoinsRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<CartPCoinsRequestMessage>(this);
            WeakReferenceMessenger.Default.Send<CartBoostRequestMessage>();
            WeakReferenceMessenger.Default.Send<CartCoinsRequestMessage>();
            WeakReferenceMessenger.Default.Send<CartPCoinsRequestMessage>();
            
            RefreshProfile();

            if(profileStore.Name == string.Empty) IsLoggedIn = false;
            else IsLoggedIn = true;
            IsLoggedInText = "Please log in";
            LoadingIsBusy = false;
        }
        public void RefreshProfile()
        {
            WeakReferenceMessenger.Default.Send<ProfileBoostRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileCoinsRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfilePCoinsRequestMessage>();
        }

        public void Receive(CartBoostAddedMessage message)
        {
            AddBoostItem(message.Boost);
        }
        public void Receive(CartCoinAddedMessage message)
        {
            AddCoinItem(message.Coins);
        }
        public void Receive(CartPCoinAddedMessage message)
        {
            AddPCoinItem(message.Premium_coins);
        }
        public void Receive(CartBoostRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            AddBoostItem(message.Response);
        }
        public void Receive(CartCoinsRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            _coins.Clear();
            foreach (Shop_Coins item in message.Response)
            {
                AddCoinItem(item);
            }
        }
        public void Receive(CartPCoinsRequestMessage message)
        {
            if (!message.HasReceivedResponse) return;
            _pcoins.Clear();
            foreach (Shop_Coins item in message.Response)
            {
                AddPCoinItem(item);
            }
        }
        private void AddBoostItem(Boost g)
        {
            _boosts.Clear();
            _boosts.Add(g);
        }
        private void AddCoinItem(Shop_Coins g)
        {
            _coins.Add(g);
        }
        private void AddPCoinItem(Shop_Coins g)
        {
            _pcoins.Add(g);
        }

        [RelayCommand]
        private Task LogIn() => Shell.Current.GoToAsync($"{nameof(LoginView)}");

        [RelayCommand]
        public async Task Buy()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                HasAgreedToTermsAndConditionsText += "!";
                return;
            }
            else
            {
                HasAgreedToTermsAndConditionsText = HasAgreedToTermsAndConditionsText.Replace("!", "");
                if(_coins.Count > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Coins bought!", "Ok");
                    foreach (Shop_Coins item in _coins) { _profileStore.AddCoins(item); }
                    _coins.Clear();
                }
                if (_pcoins.Count > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "premium coins bought!", "Ok");
                    foreach (Shop_Coins item in _pcoins) { _profileStore.AddPremiumCoins(item); }
                    _pcoins.Clear();
                }
                if (_boosts.Count > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Boost bought!", "Ok");
                    foreach (Boost item in _boosts) { _profileStore.Add_Boost(item); }
                    //WeakReferenceMessenger.Default.Send<BoostBoughtMessage>();
                    _boosts.Clear();
                }
                WeakReferenceMessenger.Default.Send<ItemsBoughtMessage>();
                HasAgreedToTermsAndConditions = false;
            }
        }
        [RelayCommand]
        Task Return() => Shell.Current.GoToAsync("..");

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

        public void Receive(CartCoinRemovedMessage message)
        {
            _coins.Remove(message.Coins);
        }

        public void Receive(CartPCoinRemovedMessage message)
        {
            _pcoins.Remove(message.Premium_coins);
        }

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
        public void Receive(UserLoggedInMessage message)
        {
            IsLoggedIn = true;
            IsLoggedInText = "already logged in";
        }
    }
    public sealed class IsTrueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return bool.TryParse(value.ToString(), out bool result) && result
                ? ValidationResult.Success
                : new ValidationResult("Must be true");
        }
    }
}
