using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.Models
{
    public partial class Shop_Coins : ObservableObject
    {
        private readonly CartStore _cartStore;
        private readonly short _factor;
        private readonly short _price;
        private readonly string _name;
        private readonly string _description;
        public short Factor { get { return _factor; } }
        public short Price { get { return _price; } }
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }


        public Shop_Coins(short factor, short price, string name, string description, CartStore cartStore)
        {
            _cartStore = cartStore;
            _name = name;
            _price = price;
            _factor = factor;
            _description = description;
        }
        public Shop_Coins(short factor, short price, string name, string description)
        {
            _cartStore = null;
            _name = name;
            _price = price;
            _factor = factor;
            _description = description;
        }

        [RelayCommand]
        private void AddCoinToCart()
        {
            _cartStore.AddCoinToCart(new Shop_Coins(Factor, Price, Name, Description,_cartStore));
        }
        [RelayCommand]
        private void AddPCoinToCart()
        {
            _cartStore.AddPCoinToCart(new Shop_Coins(Factor, Price, Name, Description,_cartStore));
        }
        [RelayCommand]
        private void RemoveCoinToCart()
        {
            _cartStore.RemoveCoinToCart(this);
        }
        [RelayCommand]
        private void RemovePCoinToCart()
        {
            _cartStore.RemovePCoinToCart(this);
        }
    }
}
