using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.Models
{
    public class CartStore : IRecipient<ItemsBoughtMessage>
    {
        private Boost _boost;
        private readonly List<Shop_Coins> _premiumCoins;
        private readonly List<Shop_Coins> _coins;
        public CartStore()
        {
            _premiumCoins = new List<Shop_Coins>();
            _coins = new List<Shop_Coins>();
            WeakReferenceMessenger.Default.Register<CartStore, CartBoostRequestMessage>(this, (r, m) => { m.Reply(r._boost); });
            WeakReferenceMessenger.Default.Register<CartStore, CartCoinsRequestMessage>(this, (r, m) => { m.Reply(r._coins); });
            WeakReferenceMessenger.Default.Register<CartStore, CartPCoinsRequestMessage>(this, (r, m) => { m.Reply(r._premiumCoins); });
            WeakReferenceMessenger.Default.Register<ItemsBoughtMessage>(this);
        }

        public void AddBoostToCart(Boost item)
        {
            _boost=item;
            WeakReferenceMessenger.Default.Send<CartBoostAddedMessage>(new CartBoostAddedMessage(item));

        }
        public void AddCoinToCart(Shop_Coins item)
        {
            _coins.Add(item);
            WeakReferenceMessenger.Default.Send<CartCoinAddedMessage>(new CartCoinAddedMessage(item));

        }
        public void AddPCoinToCart(Shop_Coins item)
        {
            _premiumCoins.Add(item);
            WeakReferenceMessenger.Default.Send<CartPCoinAddedMessage>(new CartPCoinAddedMessage(item));

        }
        public void RemoveCoinToCart(Shop_Coins item)
        {
            _coins.Remove(item);
            WeakReferenceMessenger.Default.Send<CartCoinRemovedMessage>(new CartCoinRemovedMessage(item));

        }
        public void RemovePCoinToCart(Shop_Coins item)
        {
            _premiumCoins.Remove(item);
            WeakReferenceMessenger.Default.Send<CartPCoinRemovedMessage>(new CartPCoinRemovedMessage(item));

        }
        public void Receive(ItemsBoughtMessage message)
        {
            _boost = null;
            _coins.Clear();
            _premiumCoins.Clear();
        }
    }
}
