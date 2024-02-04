using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.Models
{
    public record ProfileBoostExpiredMessage(){}
    public record ItemsBoughtMessage() { }
    public record RefreshProfileMessage() { }
    public record SaveProfileMessage() { }
    public record BoostBoughtMessage() { }
    public record UserLoggedInMessage() { }
    public record CartBoostAddedMessage(Boost g)
    {
        public Boost Boost => g;
    }
    public record CartCoinAddedMessage(Shop_Coins g)
    {
        public Shop_Coins Coins => g;
    }
    public record CartPCoinAddedMessage(Shop_Coins g)
    {
        public Shop_Coins Premium_coins => g;
    }
    public record CartCoinRemovedMessage(Shop_Coins g)
    {
        public Shop_Coins Coins => g;
    }
    public record CartPCoinRemovedMessage(Shop_Coins g)
    {
        public Shop_Coins Premium_coins => g;
    }
    public class CartBoostRequestMessage : RequestMessage<Boost> { }
    public class CartCoinsRequestMessage : RequestMessage<IEnumerable<Shop_Coins>> { }
    public class CartPCoinsRequestMessage : RequestMessage<IEnumerable<Shop_Coins>> { }
    public class ProfileBoostRequestMessage : RequestMessage<Boost> { }
    public class ProfileNameRequestMessage : RequestMessage<string> { }
    public class ProfileClickRequestMessage : RequestMessage<int> { }
    public class ProfileCoinsRequestMessage : RequestMessage<int> { }
    public class ProfilePCoinsRequestMessage : RequestMessage<int> { }

}
