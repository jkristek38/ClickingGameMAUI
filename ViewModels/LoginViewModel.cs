using ClickingGame.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ClickingGame.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ProfileStore _profileStore;
        [ObservableProperty]
        private string entryName;
        [ObservableProperty]
        private string entryPassword;
        [ObservableProperty]
        private Microsoft.Maui.Graphics.Color loginButtonBackColor;
        [ObservableProperty]
        private bool logged = true;
        [ObservableProperty]
        private string loggingButtonText = "Login";
        public LoginViewModel(ProfileStore profileStore)
        {
            LoginButtonBackColor = Colors.Blue; 
            _profileStore = profileStore;
        }
        [RelayCommand]
        public async void LoginUser()
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("User_database.json");
            var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            foreach (Json_Template2 item in JsonSerializer.Deserialize<List<Json_Template2>>(json))
            {
                SHA256 sha256Hash = SHA256.Create();

                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(EntryPassword));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                if (EntryName == item.Name && builder.ToString() == item.Password)
                {
                    LoginButtonBackColor = Colors.Green;
                    Logged = false;
                    LoggingButtonText = "Already logged";
                    EntryName = null;
                    EntryPassword = null;
                    WeakReferenceMessenger.Default.Send<UserLoggedInMessage>();
                    _profileStore.UpdateDataLoggedUser(item);
                    Return();
                }
                else
                {
                    LoginButtonBackColor = Colors.Red;
                    EntryName = null;
                    EntryPassword = null;
                }
            }
        }
        [RelayCommand]
        Task Return() => Shell.Current.GoToAsync($"..?IsLoggedIn={!Logged}");
    }
}
