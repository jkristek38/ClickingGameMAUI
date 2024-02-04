using ClickingGame.Models;
using ClickingGame.ViewModels;
using ClickingGame.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace ClickingGame;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .ConfigureLifecycleEvents(AppLifecycle => {
#if WINDOWS
             AppLifecycle.AddWindows(windows => windows
             .OnClosed((window, args) => SaveProfile()));
#endif
#if ANDROID
             AppLifecycle.AddAndroid(android => android
            .OnStop((activity) => SaveProfile()));
#endif
#if IOS
            AppLifecycle.AddiOS(ios => ios
            .WillTerminate((app) => SaveProfile()));
#endif
            })
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        static void SaveProfile() => WeakReferenceMessenger.Default.Send<SaveProfileMessage>();

        builder.Services.AddSingleton<ProfileStore>();
        builder.Services.AddSingleton<CartStore>();

        builder.Services.AddSingleton<ClickerViewModel>();
        builder.Services.AddSingleton<ClickerView>((s) => new ClickerView()
        {
            BindingContext = s.GetRequiredService<ClickerViewModel>()
        });
        builder.Services.AddSingleton<ProfileViewModel>();
        builder.Services.AddSingleton<ProfileView>((s) => new ProfileView()
        {
            BindingContext = s.GetRequiredService<ProfileViewModel>()
        });
        builder.Services.AddSingleton<ShopViewModel>();
        builder.Services.AddSingleton<ShopView>((s) => new ShopView()
        {
            BindingContext = s.GetRequiredService<ShopViewModel>()
        });
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoginView>((s) => new LoginView()
        {
            BindingContext = s.GetRequiredService<LoginViewModel>()
        });
        builder.Services.AddSingleton<CartViewModel>();
        builder.Services.AddSingleton<CartView>((s) => new CartView()
        {
            BindingContext = s.GetRequiredService<CartViewModel>()
        });
        builder.Services.AddTransient<DetailsViewModel>();
        builder.Services.AddTransient<DetailsView>((s) => new DetailsView()
        {
            BindingContext = s.GetRequiredService<DetailsViewModel>()
        });
        builder.Services.AddSingleton<SigninViewModel>();
        builder.Services.AddSingleton<SigninView>((s) => new SigninView()
        {
            BindingContext = s.GetRequiredService<SigninViewModel>()
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
