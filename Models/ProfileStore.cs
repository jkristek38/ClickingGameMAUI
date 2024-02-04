using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClickingGame.Models
{
    public class ProfileStore : IRecipient<SaveProfileMessage>
    {
        private Random random;
        private Boost _active_boosts;
        private int _coins;
        private int _premium_coins;
        private string _name;
        private int _clicks;
        public int Clicks { get { return _clicks; } set { _clicks = value; } }
        public int Coins { get { return _coins; } set { if (value + _coins > 0) _coins = value; } }
        public int Premium_Coins { get { return _premium_coins; } set { if (value + _premium_coins > 0) _premium_coins = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public Boost Active_boost { get { return _active_boosts; } set { _active_boosts = value; } }
        public ProfileStore()
        {
            random = new Random();
            _clicks = 0;
            _active_boosts = null;
            _coins = 0;
            _premium_coins = 0;
            _name = string.Empty;
            WeakReferenceMessenger.Default.Register<SaveProfileMessage>(this);
            WeakReferenceMessenger.Default.Register<ProfileStore, ProfileBoostRequestMessage>(this, (r, m) => { m.Reply(r.Active_boost); });
            WeakReferenceMessenger.Default.Register<ProfileStore, ProfileCoinsRequestMessage>(this, (r, m) => { m.Reply(r.Coins); });
            WeakReferenceMessenger.Default.Register<ProfileStore, ProfilePCoinsRequestMessage>(this, (r, m) => { m.Reply(r.Premium_Coins); });
            WeakReferenceMessenger.Default.Register<ProfileStore, ProfileClickRequestMessage>(this, (r, m) => { m.Reply(r.Clicks); });
            WeakReferenceMessenger.Default.Register<ProfileStore, ProfileNameRequestMessage>(this, (r, m) => { m.Reply(r.Name); });
        }
        public void Add_Boost(Boost b)
        {
            if (_active_boosts != null)
            {
                _active_boosts = b;
                WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
            }
        }
        public void Activate_Boost()
        {
            if (_active_boosts != null)
            {
                if (_active_boosts.IsActive == false)
                {
                    _active_boosts.ActivateBoost();
                    WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
                }
            }
        }
        public void Remove_Boost()
        {
            if (_active_boosts.IsActive == false)
            {
                _active_boosts = null;
                WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
            }
        }
        public void AddCoins(bool sure)
        {
            if (sure) Coins++;
            else {

                Coins+=random.Next(0,5) switch
                {
                    0 => -1,
                    2 => 1,
                    1 => 1,
                    3 => 1,
                    _ => 0
                };
            }
            if (_active_boosts != null)
            {
                if (_active_boosts.IsActive) Coins += _active_boosts.Factor;
            }
            WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
        }
        public void AddCoins(Shop_Coins s)
        {
            Coins += s.Factor;
            WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
        }
        public void AddPremiumCoins(bool sure)
        {
            if (sure) Premium_Coins++;
            else
            {
                Premium_Coins += random.Next(0, 5) switch
                {
                    0 => 1,
                    _ => 0
                };
            }
            if (_active_boosts != null)
            {
                if (_active_boosts.IsActive) Premium_Coins += _active_boosts.Factor;
            }
            WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
        }
        public void AddPremiumCoins(Shop_Coins s)
        {
            Premium_Coins += s.Factor;
            WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
        }
        public void AddClick()
        {
            if (_active_boosts != null)
            {
                if (Active_boost.IsActive)
                {
                    Active_boost.Time--;
                    if (Active_boost.Time == 0)
                    {
                        _active_boosts = null;
                        WeakReferenceMessenger.Default.Send<ProfileBoostExpiredMessage>(new ProfileBoostExpiredMessage());
                    }
                }
            }         
            _clicks++;
            WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
        }

        public void UpdateDataLoggedUser(Json_Template2 t)
        {
            Coins += t.Coins;
            Premium_Coins += t.Premium_Coins;
            Clicks += t.Clicks;
            Name=t.Name;
            if (Active_boost == null && t.Active_boost != null)
            {
                var r = t.Active_boost.Split(";");
                Active_boost = new Boost(int.Parse(r[0]), short.Parse(r[1]), short.Parse(r[2]), r[3], r[4]);
            }
            WeakReferenceMessenger.Default.Send<RefreshProfileMessage>();
        }

        public async void Receive(SaveProfileMessage message)
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("User_database.json");
            StreamReader reader = new StreamReader(stream);
            List<Json_Template2> _data = JsonSerializer.Deserialize<List<Json_Template2>>(await reader.ReadToEndAsync());
            for(int i=0; i< _data.Count; i++)
            {
                if (_data[i].Name == Name)
                {
                    _data[i].Coins = Coins;
                    _data[i].Premium_Coins = Premium_Coins;
                    if (Active_boost == null) _data[i].Active_boost = null;
                    else _data[i].Active_boost = Active_boost.ToString();
                    _data[i].Clicks = Clicks;
                }
            }
            await JsonSerializer.SerializeAsync(stream, _data);
            stream.Dispose();
            reader.Dispose();
        }
    }
}
