using ClickingGame.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClickingGame.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace ClickingGame.ViewModels
{
    public partial class ShopViewModel : ObservableObject, IRecipient<RefreshProfileMessage>, IRecipient<ProfileBoostRequestMessage>, IRecipient<ProfileCoinsRequestMessage>, IRecipient<ProfilePCoinsRequestMessage>
    {
        private readonly CartStore _cartStore;
        private ProfileStore _profileStore;
        private IConnectivity connection;
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
        private  ObservableCollection<Boost> _boosts;
        public ObservableCollection<Boost> Boosts => _boosts;
        private ObservableCollection<Shop_Coins> _coins;
        public ObservableCollection<Shop_Coins> NormalCoins => _coins;
        private ObservableCollection<Shop_Coins> _pcoins;
        public ObservableCollection<Shop_Coins> PCoins => _pcoins;
        public ShopViewModel(ProfileStore profileStore, CartStore cartStore/*, IConnectivity connection*/)
        {
            BoostText = "buy boost";
            _boosts = new ObservableCollection<Boost>();
            _coins = new ObservableCollection<Shop_Coins>();
            _pcoins = new ObservableCollection<Shop_Coins>();
            BoostText = "buy boost";
            HasBoostButton = Colors.LightGray;
            BoostNotActive = false;
            _profileStore = profileStore;
            _cartStore = cartStore;

            WeakReferenceMessenger.Default.Register<RefreshProfileMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileBoostRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileCoinsRequestMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfilePCoinsRequestMessage>(this);
            RefreshProfile();
            LoadDatabase();
        }
        public void RefreshProfile()
        {
            WeakReferenceMessenger.Default.Send<ProfileBoostRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfileCoinsRequestMessage>();
            WeakReferenceMessenger.Default.Send<ProfilePCoinsRequestMessage>();
        }
        private async Task LoadDatabase()
        {
           /* if (this.connection.NetworkAccess != NetworkAccess.Internet)
            {
                LoadingIsBusy = false;
                await Shell.Current.DisplayAlert("Internet Issue", "You need Internet access to buy games!", "OK");
            }
            else
            {*/
                var stream = await FileSystem.OpenAppPackageFileAsync("Boosts_database.json");
                StreamReader reader = new StreamReader(stream);
                foreach (Json_Template item in JsonSerializer.Deserialize<List<Json_Template>>(await reader.ReadToEndAsync()))
                {
                    _boosts.Add(new Boost(item.Time, item.Factor, item.Price, item.Name, item.Description, _cartStore));
                }

            stream = await FileSystem.OpenAppPackageFileAsync("Coins_database.json");
            reader = new StreamReader(stream);
            foreach (Json_Template item in JsonSerializer.Deserialize<List<Json_Template>>(await reader.ReadToEndAsync()))
            {
                _coins.Add(new Shop_Coins(item.Factor, item.Price, item.Name, item.Description, _cartStore));
            }

            stream = await FileSystem.OpenAppPackageFileAsync("PCoins_database.json");
            reader = new StreamReader(stream);
            foreach (Json_Template item in JsonSerializer.Deserialize<List<Json_Template>>(await reader.ReadToEndAsync()))
            {
                _pcoins.Add(new Shop_Coins(item.Factor, item.Price, item.Name, item.Description, _cartStore));
            }
            //}
            stream.Dispose();
            reader.Dispose();
        }
        [RelayCommand]
        private Task GoToCart() => Shell.Current.GoToAsync($"{nameof(CartView)}");

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
    }
}
