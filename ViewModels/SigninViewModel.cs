using ClickingGame.Models;
using ClickingGame.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ClickingGame.ViewModels
{
    public partial class SigninViewModel : ObservableObject
    {
        [ObservableProperty]
        private string entryName;
        [ObservableProperty]
        private string entryPassword;
        [ObservableProperty]
        private Microsoft.Maui.Graphics.Color loginButtonBackColor;
        [ObservableProperty]
        private bool logged = true;
        [ObservableProperty]
        private string loggingButtonText = "Signin";
        public SigninViewModel()
        {
            LoginButtonBackColor = Colors.Blue;
        }
        [RelayCommand]
        public async void SigninUser()
        {
            if (EntryName != "" && EntryPassword != "")
            {
                LoginButtonBackColor = Colors.Green;
                Logged = false;
                LoggingButtonText = "Already Signed in";

                SHA256 sha256Hash = SHA256.Create();

                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(EntryPassword));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                
              /*  List<Json_Template2> _data = new List<Json_Template2>();
                _data.Add(new Json_Template2()
                {
                    Name = EntryName,
                    Coins = 0,
                    Premium_Coins = 0,
                    Active_boost = null,
                    Clicks = 0,
                    Password = builder.ToString()
                });*/

                var stream = await FileSystem.OpenAppPackageFileAsync("User_database.json");
                StreamReader reader = new StreamReader(stream);
                List<Json_Template2> _data = JsonSerializer.Deserialize<List<Json_Template2>>(await reader.ReadToEndAsync());
                _data.Add(new Json_Template2()
                {
                    Name = EntryName,
                    Coins = 0,
                    Premium_Coins = 0,
                    Active_boost = null,
                    Clicks = 0,
                    Password = builder.ToString()
                });
                await JsonSerializer.SerializeAsync(stream, _data);
                stream.Dispose();
                reader.Dispose();
                EntryName = null;
                EntryPassword = null;
                Return();
            }
            else
            {
                LoginButtonBackColor = Colors.Red;
                EntryName = null;
                EntryPassword = null;
            }
        }
        [RelayCommand]
        Task Return() => Shell.Current.GoToAsync($"../{nameof(LoginView)}");
    }
}
