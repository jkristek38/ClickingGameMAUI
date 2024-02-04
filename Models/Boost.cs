using ClickingGame.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickingGame.Models
{
    public partial class Boost : ObservableObject
    {
        private readonly CartStore _cartStore;
        private readonly short _factor;
        private int _time;
        private readonly short _price;
        private bool _isActive;
        private readonly string _name;
        private readonly string _description;
        public bool IsActive { get { return _isActive; } }
        public short Factor { get { return _factor; } }
        public short Price { get { return _price; } }
        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public int Time { get { return _time; } set { _time = value; } }

        public Boost(int time, short factor,short price,string name,string description, CartStore cartStore)
        {
            _name = name;
            _isActive = false;
            _price =price;
            _factor = factor;
            _description = description;
            _time= time;
            _cartStore= cartStore;
        }
        public Boost(int time, short factor, short price, string name, string description)
        {
            _name = name;
            _isActive = false;
            _price = price;
            _factor = factor;
            _description = description;
            _time = time;
            _cartStore = null;
        }
        public void ActivateBoost() { 
            if (_time != 0) { 
                _isActive = true;
            } 
        }
        [RelayCommand]
        private void AddBoostToCart()
        {
            _cartStore.AddBoostToCart(new Boost(Time, Factor, Price,Name, Description, _cartStore));
        }

        [RelayCommand]
        private Task Details() => Shell.Current.GoToAsync($"{nameof(DetailsView)}", new Dictionary<string, object> { ["Boost"] = this });

        public override string ToString()
        {
            return (IsActive.ToString() + ";" + Factor + ";" + Price + ";" + Name + ";" + Description + ";" + Time);
        }
    }
}
